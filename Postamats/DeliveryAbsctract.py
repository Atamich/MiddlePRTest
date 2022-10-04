from abc import ABC, abstractmethod
from string import ascii_uppercase as alphabet

class DeliveryAbsctract(ABC):

    @abstractmethod
    def GetOrGenerateVCID(self, uuid):
        pass

    @abstractmethod
    def ProcessApiData(self):
        pass

    def GenerateVCNumber(self, previousId):
        """
        Generate a VC number based on the previousId
        50000
        ...
        5A102
        5A103
        ...
        5A999
        5B001
        5B002
        :param previousId: 5A999
        :return: 5B001
        """
        newId = previousId
        i = len(previousId) - 1
        nextSymbol = True

        while i >= 0 and nextSymbol:
            nextSymbol = False
            char = previousId[i]
            newChar = self.IterateVCSymbol(char)
            if newChar == False:
                newChar = '0'
                nextSymbol = True

            newId = newId[:i] + newChar + newId[i+1:]
            i -= 1
        
        return newId

    def IterateVCSymbol(self, symbol):
        if(symbol >= '0' and symbol < '9'):
            newSymbol = int(symbol)+1
        else:
            if(symbol == '9'):
                return 'A'
            if(symbol == 'Z'):
                return False
            newSymbol = alphabet[alphabet.index(symbol)+1]
        return str(newSymbol)

    def PhoneFormat(self,n):                                                                                                                                  
        return format(int(n[:-1]), ",").replace(",", "-") + n[-1]   