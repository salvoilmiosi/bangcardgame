from html.parser import HTMLParser
from pathlib import Path
import requests
import json
import os
import re

site_url = "https://bang.dvgiochi.com/"
url = "https://bang.dvgiochi.com/cardslist.php?id=5#q_result"
out_json = "bang_high_noon_fistful_cards.json"

r = requests.get(url)

def to_dict(attrs):
    attrs_dict = {}
    for k,v in attrs:
        attrs_dict[k] = v
    return attrs_dict

PARSING_NAME = 1
PARSING_COUNT = 2
PARSING_SIGNS = 3

all_cards = []

class MyHTMLParser(HTMLParser):
    parsing_card = False
    nesting_level = 0
    current_card = {}
    current_sign = ''
    parsing = 0

    def handle_starttag(self, tag, attrs):
        attrs_dict = to_dict(attrs)
        if tag == 'div':
            if 'class' in attrs_dict:
                if 'card-box' in attrs_dict['class']:
                    self.parsing_card = True
                    self.nesting_level = 0
                    self.current_card = {}
                    self.parsing = 0
                elif self.parsing_card:
                    if attrs_dict['class'] == 'c-tit':
                        self.parsing = PARSING_NAME
                    elif attrs_dict['class'] == 'c-num':
                        self.parsing = PARSING_COUNT
                    elif attrs_dict['class'] == 'c-segni':
                        self.parsing = PARSING_SIGNS
                        self.current_card['signs'] = []
                    else:
                        self.parsing = 0
                    self.nesting_level += 1
                else:
                    self.parsing_card = False
        elif tag == 'img' and self.parsing_card and 'src' in attrs_dict:
            if self.nesting_level == 0:
                self.current_card['image'] = attrs_dict['src']
            elif self.nesting_level == 3 and self.current_sign != '':
                if 'i_c' in attrs_dict['src']:
                    self.current_card['signs'].append(self.current_sign + 'C')
                elif 'i_q' in attrs_dict['src']:
                    self.current_card['signs'].append(self.current_sign + 'Q')
                elif 'i_f' in attrs_dict['src']:
                    self.current_card['signs'].append(self.current_sign + 'F')
                elif 'i_p' in attrs_dict['src']:
                    self.current_card['signs'].append(self.current_sign + 'P')
                self.current_sign = ''
    
    def handle_endtag(self, tag):
        if self.parsing_card and tag == 'div':
            self.parsing = 0
            if self.nesting_level == 0:
                if not self.current_card['signs']:
                    self.current_card.pop('signs')
                all_cards.append(self.current_card)
                self.parsing_card = False
            else:
                self.nesting_level -= 1
    
    def handle_data(self, data):
        if self.parsing_card:
            if self.parsing == PARSING_NAME:
                self.current_card['name'] = data
            elif self.parsing == PARSING_COUNT:
                p = re.compile('copie nel gioco: x([0-9]+)')
                m = p.match(data)
                if m:
                    self.current_card['count'] = m.group(1)
            elif self.parsing == PARSING_SIGNS:
                if data[0] == '-':
                    data = data[1:]
                self.current_sign = data

parser = MyHTMLParser()
parser.feed(r.text)

with open(out_json,'w') as content_file:
    content_file.write(json.dumps(all_cards))

for card in all_cards:
    r = requests.get(site_url + card['image'])
    path = Path(card['image'])
    if not path.parent.exists():
        os.makedirs(path.parent)
    print(card['image'])
    with open(card['image'], 'wb') as content_file:
        content_file.write(r.content)
