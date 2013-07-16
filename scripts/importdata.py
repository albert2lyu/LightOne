#!/usr/bin/env python
import glob
import os
import time

zip_files = sorted(glob.glob('./queen-dump-*.7z'))
for zip_file in zip_files:
        os.system('7za e ' + zip_file + ' -y -o./tmp')
        for json_file in os.listdir('./tmp'):
                collection, ext = os.path.splitext(json_file)
                command = 'mongoimport --upsert -db queen -c ' + collection + ' ./tmp/' + json_file
                os.system(command)
                os.remove('./tmp/' + json_file)
        os.remove(zip_file)

if len(zip_files) == 0:
        time.sleep(60)
