#!/usr/bin/env python3

import sys
import secrets


class Main:
    
    def __init__(self):
        if len(sys.argv) != 5 or sys.argv[1] not in ["-E","-D","-I","-R"]  :
            self.PrintUsage()
            
        elif(sys.argv[1] == "-E"):
            self.Encrypt(sys.argv[2],sys.argv[3],sys.argv[4])
        elif(sys.argv[1] == "-D"):
            self.Decrypt(sys.argv[2],sys.argv[3],sys.argv[4])
        elif(sys.argv[1] == "-I"):
            self.Interlace(sys.argv[2],sys.argv[3],sys.argv[4])
        elif(sys.argv[1] == "-R"):
            self.Restore(sys.argv[2],sys.argv[3],sys.argv[4])
            
    
    def ReadFileBytes(self,path):
        allBytes = []
        with open(path, "rb") as f:
            while (byte := f.read(1)):
                allBytes.append(byte)
        return allBytes
    
    
    def SaveFileBytes(self,path, byteFile):
        with open(path, "wb") as binary_file:
            for b in byteFile:
                binary_file.write(b)
        
        
    def GenerateRandomBlock(self, lenght):
        result = []
        for _ in range(lenght):
            result.append( bytes( secrets.token_bytes(1)) )
        return result
    
    
    def GenerateZeroBlock(self,lenght):
        result = []
        for _ in range(lenght):
            result.append( bytes(1) )
        return result
    
    
    def StripZeros(self,byteblock):
        zeros = 0
        for i in range(len(byteblock)):
            if byteblock[ -1 + i] == bytes(1):
                zeros += 1        
        if zeros > 0:
            return byteblock[0:-zeros]
        return byteblock
    
    
    def Xor2Blocks(self, block1, block2):
        if(len(block1) != len(block2) ):
            print("Error: xor blocks have different sizes")
            sys.exit(-1)
            
        result = []
        for i in range(len(block1)):
            result.append( self.bXor( block1[i] , block2[i] )  )
        return result
    
    
    def bXor(self, ba1, ba2):
        """ XOR two byte strings """
        return bytes([_a ^ _b for _a, _b in zip(ba1, ba2)])
            
            
            
    def PrintUsage(self):
        print("""
Usage:
    -E <Inputfile> <Outputfile-1> <Ouputfile-2>        Encrypts a file into 2 secure blocks
    -D <Inputfile-1> <Inputfile-2> <OuputFile>         Decrypts a file from 2 blocks
    -I <Inputfile-1> <InputFile-1> <Ouputfile>         Interlaces 2 files (RAID)
    -R <Inputfile-1> <InputFile-1> <Ouputfile>         Restores a file from 2 interlaces
""")
        
        
    def Encrypt(self,pathInputFile,destinationFile1,destinationFile2):
        fileBytes = self.ReadFileBytes(pathInputFile)
        randomBytes = self.GenerateRandomBlock( len(fileBytes) )
        resultBlock = self.Xor2Blocks(fileBytes,randomBytes)
        self.SaveFileBytes(destinationFile1,resultBlock)
        self.SaveFileBytes(destinationFile2,randomBytes)
        
        
    def Decrypt(self,pathInputFile1,pathInputFile2,destinationFile):
        file1 = self.ReadFileBytes(pathInputFile1)
        file2 = self.ReadFileBytes(pathInputFile2)
        result = self.Xor2Blocks(file1, file2)
        self.SaveFileBytes(destinationFile,result)
        
        
    def Interlace(self,pathInputFile1,pathInputFile2,destinationFile):
        
        file1 = self.ReadFileBytes(pathInputFile1)
        file2 = self.ReadFileBytes(pathInputFile2)
                
        if len(file1) > len(file2):
            file2 += self.GenerateZeroBlock( len(file1) - len(file2) )
        elif len(file1) < len(file2):
            file1 += self.GenerateZeroBlock( len(file2) - len(file1) )
                
        result = self.Xor2Blocks(file1, file2)
        self.SaveFileBytes(destinationFile,result)
    
        
    def Restore(self,pathInputFile1,pathInputFile2,destinationFile):
        
        file1 = self.ReadFileBytes(pathInputFile1)
        file2 = self.ReadFileBytes(pathInputFile2)
                
        if len(file1) > len(file2):
            file2 += self.GenerateZeroBlock( len(file1) - len(file2) )
        elif len(file1) < len(file2):
            file1 += self.GenerateZeroBlock( len(file2) - len(file1) )
            
        result = self.StripZeros( self.Xor2Blocks(file1, file2) )
        self.SaveFileBytes( destinationFile, result )


if __name__ == '__main__':
    m = Main()