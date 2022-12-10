from flask import Flask, request
from flask_restful import Resource, Api

from tinydb import TinyDB, Query
from tinydb.storages import JSONStorage
from tinydb.middlewares import CachingMiddleware

import sys
import json

app = Flask(__name__)
api = Api(app)

dataBaseFile = "C:\\Users\\kim.gundersen\\Dropbox\\MiniDbServer\\DefaultMiniDatabase.mdb.json"
try: f = open(dataBaseFile,"w+").close()
except Exception as ex:
    print( ex ) 
    sys.exit(-1)





class Insert(Resource):
    def get(self,tableName):
        with TinyDB(dataBaseFile) as db:
            table = db.table(tableName)
            table.insert( request.form['data']  )

        return 

class CreateTable(Resource):
    def post(self,tableName):
        with TinyDB(dataBaseFile) as db:
            table = db.table(tableName)
            table.insert( {"value": True } )
        return {'Message: ': 'Table created'}, 200

class Query(Resource):
    def post(self,tableName):
        pass

api.add_resource(CreateTable, '/<string:tableName>/CreateTable')
api.add_resource(Insert, '/<string:tableName>/InsertJson')
api.add_resource(Query, '/<string:tableName>/Query')

if __name__ == '__main__':
    app.run(debug=True)