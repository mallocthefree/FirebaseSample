##########################################################
# <copyright company="Jeremy Snyder Consulting">
# Copyright (c) 2021 All Rights Reserved
# </copyright>
# <author>Jeremy Snyder</author>
# <date>03/23/2021</date>
##########################################################

import os
import io

directory = os.path.dirname(os.path.realpath(__file__))
os.chdir(directory)

# Change the order of folders to ensure the order in which they should appear in the master SQL file
subfolderList =\
                [
                    "../SQL",
                    "../SQL/security",
                    "../SQL/environment",
                    "../SQL/functions/_global",
                    "../SQL/schemas",
                    "../SQL/pre",
                    "../SQL/tables",
                    "../SQL/fks",
                    "../SQL/static",
                    "../SQL/functions",
                    "../SQL/procedures",
                    "../SQL/post",
                    "../SQL/testData"
                ]

print("==================================================")
print("Building master SQL file for the database")
print("File list:")

with io.open('Clear_Master.sql', 'wt', encoding="utf-8") as outfile:
    for folder in subfolderList:
        if os.path.exists(folder):
            file_names = sorted(os.listdir(folder))
            for filename in file_names:
                if filename.endswith(".sql"):
                    path = folder + "/" + filename
                    with io.open(path, "rt", encoding="utf-8") as f:
                        print(path)
                        content = f.read()
                        outfile.write(content)

outfile.close()

print("==================================================")
print("Completed master database script")
print("==================================================")
