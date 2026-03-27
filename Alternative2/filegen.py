#!/usr/bin/env python3
"""
Faster generator for a large noise grid that contains exactly the specified hidden words
and no other dictionary words in any of 8 directions.

Configure the top section and run. Use TEST_MODE True for quick iteration.
"""
import random
from collections import defaultdict
from typing import Dict, List, Tuple

# ---------------- CONFIG ----------------
TEST_MODE = True
ROWS = 10000 if not TEST_MODE else 200
COLS = 1000 if not TEST_MODE else 200
DICT_PATH = "words.txt"                 # one lowercase word per line
WORDS_TO_HIDE = ["alpha", "bravo", "charlie"]
MIN_WORD_LEN = 3
MAX_WORD_LEN = 12
ALPHABET = "abcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()-_=+[]{};:,.<>/?"
OUTPUT_PATH = "big_noise_grid.txt"
BLOCK_SIZE = 200 if not TEST_MODE else 50
MAX_TRIES_PER_CELL = 200
MAX_BLOCK_RESTARTS = 50
# ----------------------------------------

DIRECTIONS = [(-1, 0), (1, 0), (0, -1), (0, 1),
              (-1, -1), (-1, 1), (1, -1), (1, 1)]

random.seed()

# ---------------- Trie Implementation ----------------
class TrieNode:
    __slots__ = ("children", "is_word")
    def __init__(self):
        self.children: Dict[str, "TrieNode"] = {}
        self.is_word: bool = False

class Trie:
    def __init__(self):
        self.root = TrieNode()
    def insert(self, word: str):
        node = self.root
        for ch in word:
            if ch not in node.children:
                node.children[ch] = TrieNode()
            node = node.children[ch]
        node.is_word = True
    def is_word_seq(self, seq: List[str]) -> bool:
        node = self.root
        for ch in seq:
            if ch not in node.children:
                return False
            node = node.children[ch]
        return node.is_word

# ---------------- Utilities ----------------
def load_dictionary(path: str, min_len: int, max_len: int) -> Trie:
    trie = Trie()
    with open(path, "r", encoding="utf-8") as f:
        for line in f:
            w = line.strip().lower()
            if min_len <= len(w) <= max_len and w.isalpha():
                trie.insert(w)
    return trie

def place_word_positions(rows, cols, word, occupied):
    L = len(word)
    attempts = 0
    while attempts < 5000:
        attempts += 1
        dy, dx = random.choice(DIRECTIONS)
        if dy == 0:
            y = random.randrange(0, rows)
            x = random.randrange(0, cols - (L - 1) * abs(dx)) if dx > 0 else random.randrange(L - 1, cols)
        elif dx == 0:
            x = random.randrange(0, cols)
            y = random.randrange(0, rows - (L - 1) * abs(dy)) if dy > 0 else random.randrange(L - 1, rows)
        else:
            y = random.randrange(0, rows - (L - 1)) if dy > 0 else random.randrange(L - 1, rows)
            x = random.randrange(0, cols - (L - 1)) if dx > 0 else random.randrange(L - 1, cols)
        coords = []
        yy, xx = y, x
        ok = True
