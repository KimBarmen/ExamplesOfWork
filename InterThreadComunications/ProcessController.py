from GuiView import GuiView
from GuiController import GuiController
from WorkerProcess import WorkerProcess
from multiprocessing import Process, SimpleQueue, Array


class ProcessController:

    def __init__(self):
        
        processComunicationQueue = SimpleQueue() # Comunication by passing objects
        processComunicationDict = Array('i',64) # Comunication by int statuses, defined in "Constants.py"

        # GUI
        controller = GuiController(processComunicationQueue,processComunicationDict)
        self.GuiViewProcess = GuiView(controller)

        # Worker
        worker  = WorkerProcess(processComunicationQueue,processComunicationDict)
        self.WorkerProcess = Process( target=worker.Run, )
    

    def Start(self):
        self.WorkerProcess.start()    

        # Tkinter window-draw-loop. THIS IS BLOCKING, but has its own subprocess
        # that loops GuiController.MainloopOnce forever using after() calls.
        # First after() is shedueled from GuiView.__init__.
        # Must be called from main-thread
        self.GuiViewProcess.mainloop()
    
    def KillAll(self):
        pass

    