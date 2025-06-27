import os
import tkinter.filedialog
import PIL.Image


def main():
    while True:
        imag_paths = tkinter.filedialog.askopenfilenames()

        if imag_paths == "":
            break

        for path in imag_paths:
            imag = PIL.Image.open(path)
            converted_imag = imag.convert("RGB")
            converted_imag.save(path + ".eps", lossless = True)

if __name__ == "__main__":
    main()
