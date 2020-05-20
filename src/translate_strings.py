import re
import random
import string
import sys

def random_identifier(length):
  letters = string.ascii_letters + string.digits
  return ''.join(random.choice(letters) for i in range(length))

exclude_list = ["0.0", "0.000", "Light.CombatModCollection_v1", "Combat Mod Collection", "CombatModCollection"]

def replace_string(filename):
  string_dict = dict()
  with open("output.cs", "wt") as out:
    with open(filename, "rt") as f:
      for line in f:
        origins = re.findall(r'"(.+?)"', line)
        for true_origin in origins:
          origins = true_origin.split("/")
          for origin in origins:
            if origin in exclude_list:
              continue
            if origin in string_dict:
              line = line.replace(origin, '{=' + string_dict[origin] + '}' + origin, 1)
            else:
              identifier = random_identifier(6)
              line = line.replace(origin, '{=' + identifier+ '}' + origin, 1)
              string_dict[origin] = identifier
        out.write(line)

  with open("module_strings.xml", "wt") as out:
    for key, value in string_dict.items():
      out.write('<string id="{id}" text="{text}"/>\n'.format(id=value, text=key))


if __name__ == "__main__":
  #print(random_identifier(int(sys.argv[1])))
  replace_string(sys.argv[1])
