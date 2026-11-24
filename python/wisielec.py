# Dawid Sobota 78618
# Gra Wisielec IT w Pythonie (klasy WordBank, Player, Game + plik slowa.txt)

import os
import random


class WordBank:
    """Wczytuje listę słów z pliku i losuje hasło do gry."""

    def __init__(self, filename: str = "slowa.txt", words=None):
        if words is not None:
            self.words = words
        else:
            self.words = self._load_words(filename)

    def _load_words(self, filename: str):
        # Szuka pliku w bieżącym folderze lub poziom wyżej (gdy uruchamiamy z folderu python/)
        candidates = [
            filename,
            os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", filename),
        ]
        for path in candidates:
            if os.path.exists(path):
                with open(path, "r", encoding="utf-8") as f:
                    words = [line.strip().lower() for line in f if line.strip()]
                if words:
                    return words
        raise FileNotFoundError(f"Nie znaleziono pliku ze słowami: {filename}")

    def get_random_word(self) -> str:
        return random.choice(self.words).lower()


class Player:
    """Reprezentuje gracza, jego podane litery oraz liczbę błędów."""

    def __init__(self, name: str):
        self.name = name
        self.guessed_letters = []
        self.mistakes = 0

    def has_guessed(self, letter: str) -> bool:
        return letter in self.guessed_letters

    def add_guess(self, letter: str):
        if letter not in self.guessed_letters:
            self.guessed_letters.append(letter)

    def add_mistake(self):
        self.mistakes += 1


class Game:
    """Zarządza przebiegiem gry Wisielec IT: losowaniem słowa, limitami i stanem gry."""

    def __init__(self, word_bank: WordBank, player: Player, max_mistakes: int = 6):
        self.word_bank = word_bank
        self.player = player
        self.max_mistakes = max_mistakes
        self.secret_word = self.word_bank.get_random_word()

    def get_masked_word(self) -> str:
        # Odsłania odgadnięte litery, pozostałe zastępuje znakiem '_'
        return " ".join(
            ch if self.player.has_guessed(ch) else "_"
            for ch in self.secret_word
        )

    def is_won(self) -> bool:
        return all(self.player.has_guessed(ch) for ch in self.secret_word)

    def is_lost(self) -> bool:
        return self.player.mistakes >= self.max_mistakes

    def is_game_over(self) -> bool:
        return self.is_won() or self.is_lost()

    def get_state(self) -> str:
        if self.is_won():
            return "Wygrana"
        if self.is_lost():
            return "Przegrana"
        return "W trakcie"

    def guess_letter(self, letter: str):
        letter = letter.lower()

        if self.is_game_over():
            print("Gra jest już zakończona.")
            return

        if self.player.has_guessed(letter):
            print(f"Litera '{letter}' była już podana!")
            return

        self.player.add_guess(letter)

        if letter in self.secret_word:
            print(f"Dobrze! Litera '{letter}' znajduje się w haśle.")
        else:
            self.player.add_mistake()
            print(f"Pudło! Brak litery '{letter}' w haśle.")

    def display_status(self):
        print("\n----------------------------")
        print(f"Gracz: {self.player.name} | Stan gry: {self.get_state()}")
        print(f"Hasło: {self.get_masked_word()}")
        print(f"Błędy: {self.player.mistakes} / {self.max_mistakes}")
        podane = ", ".join(self.player.guessed_letters) if self.player.guessed_letters else "brak"
        print(f"Podane litery: {podane}")

    def play(self):
        print(f"Witaj w grze Wisielec IT, {self.player.name}!")

        while not self.is_game_over():
            self.display_status()
            tekst = input("Podaj literę: ").strip().lower()

            if len(tekst) != 1 or not tekst.isalpha():
                print("Wpisz dokładnie jedną literę!")
                continue

            self.guess_letter(tekst)

        self.display_status()
        if self.is_won():
            print(f"Gratulacje! Odgadłeś hasło: {self.secret_word}")
        else:
            print(f"Koniec gry! Ukryte hasło to: {self.secret_word}")


if __name__ == "__main__":
    bank = WordBank("slowa.txt")
    imie = input("Podaj imię gracza: ").strip() or "Gracz"
    player = Player(imie)
    game = Game(bank, player, max_mistakes=6)
    game.play()
