import time
import tkinter as tk
from tkinter import StringVar, Variable, ttk

import Constants as C
from GuiController import GuiController

class GuiView(tk.Tk):

    MainloopMsDelay = 30
    FpsLabelValue = None
    fps = 30

    def __init__(self, Controller):
        super().__init__()
        self.Controller = Controller
        self.Controller.SetView(view = self)

        sizeStr = "{0}x{1}".format( C.DEFAULT_SCREEN_WIDTH, C.DEFAULT_SCREEN_HEIGHT )
        self.geometry(sizeStr)
        self.title('Random snake')
        #self.resizable(0, 0)

        self.Create_widgets()
        self.SetTheme()
        
        self.after( self.MainloopMsDelay , self.GuiControllerLoop)
    
        self.FpsLabelValue = tk.StringVar()
        self.FpsLabelValue.set("--FPS--")

    def btn_pause(self): 
        self.Controller.btn_pause()

    def Create_widgets(self):
        # Top Button
        pause_button = ttk.Button(self, text="Pause", command=self.btn_pause)
        pause_button.grid(column=1, row=0, sticky=tk.EW, padx=5, pady=5)

        # Canvas
        canvas = tk.Canvas(self, bg="lightgray", height=C.CANVAS_HEIGHT, width=C.CANVAS_WIDTH)
        canvas.grid( column=0, row=1, sticky=tk.S ,padx=5, pady=5, columnspan=3)
        self.canvas = canvas

        #FPS label
        fps_label = tk.Label(self, bg="white" ,height=1, width=10 ,textvariable = self.FpsLabelValue  )
        fps_label.grid(column=0, row=2, sticky=tk.W, padx=5, pady=5, columnspan=1 )
        self.fps_label = fps_label
        
    def SetTheme(self):
        self.style = ttk.Style(self)
        avalableThemes = self.style.theme_names()
        apropriateThemes = ["xpnative", "vista"]

        print( [v for v in avalableThemes] )

        for th in list( set(avalableThemes) & set(apropriateThemes)):
            self.style.theme_use(th)
            return


    def GuiControllerLoop(self):
        timeStart = time.time()
        self.update_idletasks()
        self.Controller.MainloopOnce()
        self.after( int(self.MainloopMsDelay) , self.GuiControllerLoop)
        self.fps = 1.0 / ((time.time() - timeStart) + (self.MainloopMsDelay *0.001)) 
