import Constants as C

from multiprocessing import freeze_support
from ProcessController import ProcessController

class Main:


    def __init__(self):
        print("Hello World")

        pc = ProcessController()
        print( "Starting ...")
        pc.Start()
        pc.KillAll()

if __name__ == '__main__':
    freeze_support()
    m = Main()