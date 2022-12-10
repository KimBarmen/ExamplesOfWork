from multiprocessing import SimpleQueue, Array
import Constants as C

class GuiController:

    """ 
    WARNING
    MainloopOnce IN THIS CLASS IS RUN BY GUI WINDOW DRAW THREAD
    AND SHOULD NOT BE DOING EXTENSIVE WORK, IDEALY ONLY
    READING FROM QUEQE AND UPDAING GUI
    """

    pointsOnCanvas = []


    def __init__(self, processComunicationQueue: SimpleQueue, processComunicationDict: Array):
        self.processComunicationQueue = processComunicationQueue
        self.processComunicationDict = processComunicationDict

    def SetView(self,view):
        self.view = view
    
    # Called by GuiView( in loop )
    def MainloopOnce(self):
        
        self.ReadFromQueue()
        self.UpdateCanvas()
        self.SetFpsLabel()


    def ReadFromQueue(self):
        while not self.processComunicationQueue.empty():
            self.pointsOnCanvas.append( self.processComunicationQueue.get() )

        while len(self.pointsOnCanvas) > C.CANVAS_MAX_POINTS:
            self.pointsOnCanvas = self.pointsOnCanvas[1:]

    def SetFpsLabel(self):
        self.view.fps_label.configure(text= "FPS: " + str(int(self.view.fps) )) 

    def btn_pause(self):
        if self.processComunicationDict[C.THREAD_ID_MAIN] == C.THREAD_COMMAND_PAUSE:
            self.processComunicationDict[C.THREAD_ID_MAIN] = C.THREAD_COMMAND_START
        else:
            self.processComunicationDict[C.THREAD_ID_MAIN] = C.THREAD_COMMAND_PAUSE


    def UpdateCanvas(self):
        self.view.canvas.delete("all")
        
        if len(self.pointsOnCanvas) > 2:
            (x1,y1) = self.pointsOnCanvas[0]
            for point in self.pointsOnCanvas[1:]:
                (x0,y0),(x1,y1) = (x1,y1),point
                self.view.canvas.create_line(x0,y0,x1,y1, fill="Red", width=1)
                