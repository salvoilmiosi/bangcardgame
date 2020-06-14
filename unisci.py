import json

out_file = {"cards":[],"characters":[],"roles":[],"highnoon":[],"fistfulofcards":[],"wildwestshow":[],"goldrush":[]}

def add_cards(values, expansion):
    for card in values["carte"]:
        card.pop('count')
        card['expansion'] = expansion
        out_file['cards'].append(card)

def add_characters(values, expansion):
    for card in values["personaggi"]:
        card.pop('count')
        card['hp'] = 4
        card['expansion'] = expansion
        out_file['characters'].append(card)

def add_roles(values, expansion):
    for card in values["ruoli"]:
        card.pop('count')
        card['expansion'] = expansion
        out_file['roles'].append(card)

def add_expansion(values, expansion, pop_count=True):
    for card in values[expansion]:
        if pop_count:
            card.pop('count')
        out_file[expansion].append(card)

with open('bang_base.json', 'r') as content_file:
    values = json.loads(content_file.read())
    add_cards(values, 'base')
    add_characters(values, 'base')
    add_roles(values, 'base')

with open('bang_dodge_city.json', 'r') as content_file:
    values = json.loads(content_file.read())
    add_cards(values, 'dodgecity')
    add_characters(values, 'dodgecity')
    add_roles(values, 'dodgecity')

with open('bang_valley_of_shadows.json', 'r') as content_file:
    values = json.loads(content_file.read())
    add_cards(values, 'valleyofshadows')
    add_characters(values, 'valleyofshadows')

with open('bang_high_noon_fistful_cards.json', 'r') as content_file:
    values = json.loads(content_file.read())
    add_expansion(values, 'fistfulofcards')
    add_expansion(values, 'highnoon')

with open('bang_wild_west_show.json', 'r') as content_file:
    values = json.loads(content_file.read())
    add_expansion(values, 'wildwestshow')
    add_characters(values, 'wildwestshow')

with open('bang_armed_dangerous.json', 'r') as content_file:
    values = json.loads(content_file.read())
    add_cards(values, 'armedanddangerous')
    add_characters(values, 'armedanddangerous')

with open('bang_gold_rush.json', 'r') as content_file:
    values = json.loads(content_file.read())
    add_expansion(values, 'goldrush', pop_count=False)
    add_characters(values, 'goldrush')
    add_roles(values, 'goldrush')

with open('bang_tutte_carte.json', 'w') as content_file:
    content_file.write(json.dumps(out_file))