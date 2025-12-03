import csv
import re

# Словник для перекладу categorій
category_mapping = {
    'Dairy products': 'DAIRY',
    'Fats, Oils, Shortenings': 'OTHER',
    'Meat, Poultry': 'MEAT',
    'Fish, Seafood': 'FISH',
    'Vegetables': 'VEGETABLES',
    'Fruits': 'FRUITS',
    'Grain Products': 'GRAINS',
    'Bakery Products': 'GRAINS',
    'Beverages': 'BEVERAGES',
    'Desserts': 'SNACKS',
    'Nuts': 'SNACKS',
    'Sweets': 'SNACKS'
}

# Словник для перекладу назв продуктів
translations = {
    # Dairy
    "Milk": "Молоко",
    "Cheese": "Сир",
    "Butter": "Масло",
    "Cream": "Вершки",
    "Yogurt": "Йогурт",
    "Ice cream": "Морозиво",
    "Cottage cheese": "Творог",
    "Sour cream": "Сметана",
    "Buttermilk": "Бутермілк",
    
    # Meat
    "Beef": "Яловичина",
    "Pork": "Свинина",
    "Chicken": "Курка",
    "Turkey": "Індичка",
    "Lamb": "Баранина",
    "Duck": "Качка",
    "Bacon": "Бекон",
    "Ham": "Шинка",
    "Sausage": "Ковбаса",
    
    # Fish
    "Salmon": "Лосось",
    "Tuna": "Тунець",
    "Cod": "Тріска",
    "Fish": "Риба",
    "Crab": "Краб",
    "Shrimp": "Креветки",
    
    # Vegetables
    "Potato": "Картопля",
    "Tomato": "Помідор",
    "Carrot": "Морква",
    "Onion": "Цибуля",
    "Cabbage": "Капуста",
    
    # Fruits
    "Apple": "Яблуко",
    "Banana": "Банан",
    "Orange": "Апельсин",
    "Grape": "Виноград",
    
    # Grains
    "Bread": "Хліб",
    "Rice": "Рис",
    "Pasta": "Макарони",
    
    # Eggs
    "Egg": "Яйце",
    "Eggs": "Яйця"
}

def translate_food_name(name):
    """Переклад назви продукту"""
    for eng, ukr in translations.items():
        if eng.lower() in name.lower():
            return name.replace(eng, ukr)
    return name

def get_category(original_category):
    """Отримання категорії"""
    return category_mapping.get(original_category, 'OTHER')

def calculate_per_100g(calories, protein, fat, carbs, grams):
    """Розрахунок на 100г"""
    if grams == 0:
        return 0, 0, 0, 0
    
    factor = 100.0 / grams
    return (
        round(calories * factor, 2),
        round(protein * factor, 2),
        round(fat * factor, 2),
        round(carbs * factor, 2)
    )

# Читаємо оригінальний файл
products = []
with open('nutrients.csv', 'r', encoding='utf-8') as f:
    reader = csv.DictReader(f)
    for row in reader:
        try:
            # Парсимо дані
            food_name = row['Food'].strip()
            grams = float(row['Grams'].replace(',', ''))
            calories = float(row['Calories'].replace(',', ''))
            protein = float(row['Protein']) if row['Protein'] not in ['t', ''] else 0.5
            fat = float(row['Fat']) if row['Fat'] not in ['t', ''] else 0.5
            carbs = float(row['Carbs']) if row['Carbs'] not in ['t', ''] else 0.5
            category = row['Category'].strip()
            
            # Розраховуємо на 100г
            cal_100g, prot_100g, fat_100g, carb_100g = calculate_per_100g(
                calories, protein, fat, carbs, grams
            )
            
            # Перевіряємо, що значення в допустимих межах
            if cal_100g > 900 or prot_100g > 100 or fat_100g > 100 or carb_100g > 100:
                continue
            
            # Перекладаємо назву та категорію
            ukrainian_name = translate_food_name(food_name)
            mapped_category = get_category(category)
            
            products.append([
                ukrainian_name,
                mapped_category,
                cal_100g,
                prot_100g,
                fat_100g,
                carb_100g
            ])
            
        except (ValueError, KeyError) as e:
            continue

# Обмежуємо до 500 продуктів
products = products[:500]

# Записуємо у products.csv
with open('products.csv', 'w', newline='', encoding='utf-8') as file:
    writer = csv.writer(file)
    writer.writerow(['Name', 'Category', 'CaloriesPer100g', 'ProteinPer100g', 'FatPer100g', 'CarbsPer100g'])
    for product in products:
        writer.writerow(product)

print(f"✓ Оброблено {len(products)} реальних продуктів з USDA датасету")
print(f"✓ Дані збережено в products.csv")
