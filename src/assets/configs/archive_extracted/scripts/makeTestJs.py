import os
import re
import sys

cur_path = sys.argv[1]
list_files = open(cur_path + '\\' + 'main.js').read().split(',')

for i in range(len(list_files)):
    list_files[i] = re.search(r".*'(.*)'", list_files[i]).group(1)

test_js = open(cur_path + '\\' + 'test_temp.js', "w")

for file in list_files:
    text = open(cur_path + '\\' + file).read()
    test_js.write('//' + file + '\n' + text + '\n')
