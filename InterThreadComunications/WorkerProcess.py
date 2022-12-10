import Constants as C

import time
from random import Random 


class WorkerProcess:

    r = Random()

    def __init__(self,processComunicationQueue,processComunicationDict):
        self.processComunicationQueue = processComunicationQueue
        self.processComunicationDict = processComunicationDict


    def Run(self): 
        aimX,aimY = C.CANVAS_WIDTH / 2, C.CANVAS_HEIGHT / 2
        ghostX, ghostY = aimX,aimY
        posX,posY = aimX,aimY

        while True:
            ghostX = self.NewPoints(ghostX,aimX, C.CALCULATOR_GRAVETYWELL)
            ghostY = self.NewPoints(ghostY,aimY,C.CALCULATOR_GRAVETYWELL)
            posX = self.NewPoints(posX,ghostX,C.CALCULATOR_STUBBORN) 
            posY = self.NewPoints(posY,ghostY,C.CALCULATOR_STUBBORN) 

            self.processComunicationQueue.put( (posX,posY) )
            while self.CheckProcessComunicationDict(self.processComunicationDict):
                time.sleep(C.CALCULATOR_DELAY * 10)
            time.sleep(C.CALCULATOR_DELAY)
    

    def CheckProcessComunicationDict(self,dict):
        if( dict[C.THREAD_ID_MAIN] == C.THREAD_COMMAND_PAUSE):
            return True
        return False


    def NewPoints(self,pos,target, multplier):
        c1 = self.r.randint(-C.CALCULATOR_RANDOM_AMPLITUDE,C.CALCULATOR_RANDOM_AMPLITUDE)
        return pos + c1 + ((target-pos) * multplier)