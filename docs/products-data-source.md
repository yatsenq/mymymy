# Products Data Source

## Source
**User Provided Dataset (Ukrainian)**
- Comprehensive list of common Ukrainian food products
- Includes accurate nutritional values (Calories, Protein, Fat, Carbs)
- Categorized by food groups (Grains, Meat, Fish, Dairy, Vegetables, Fruits, Snacks, Beverages, Other)

## Description
The dataset contains nutritional information for 244 food items.

Each entry includes:
- **Food Name** (in Ukrainian)
- **Category**: GRAINS, MEAT, FISH, DAIRY, VEGETABLES, FRUITS, SNACKS, BEVERAGES, OTHER
- **Calories** (per 100g)
- **Protein** (per 100g)  
- **Fat** (per 100g)
- **Carbohydrates** (per 100g)

## Import Process

1. **Data Preparation**: Converted user-provided SQL INSERT statements into a CSV format using a Python script.
2. **CSV Generation**: Created `products.csv` containing the structured data.
3. **Import Script**: C# script [import-products.cs](file:///Users/romanzabolotnii/Desktop/Programming/програмна%20інженерія/StayFit/src/database/import-products.cs) reads the CSV and bulk inserts into PostgreSQL.
4. **Execution**: Integrated into [Program.cs](file:///Users/romanzabolotnii/Desktop/Programming/програмна%20інженерія/StayFit/src/database/Program.cs) to run on application startup.

## Schema Mapping

| CSV Column | Database Column | Type | Notes |
|------------|-----------------|------|-------|
| Name | name | VARCHAR | Ukrainian names |
| Category | category | VARCHAR | Mapped to database enums |
| CaloriesPer100g | calories_per_100g | DECIMAL | |
| ProteinPer100g | protein_per_100g | DECIMAL | |
| FatPer100g | fat_per_100g | DECIMAL | |
| CarbsPer100g | carbs_per_100g | DECIMAL | |

## Product Examples

**Grains:**
- Гречка (суха) - 313 kcal
- Рис білий (сухий) - 344 kcal

**Meat:**
- Куряче філе (сире) - 113 kcal
- Яловичина (вирізка) - 158 kcal

**Dairy:**
- Молоко 2.5% - 52 kcal
- Сир кисломолочний 5% - 121 kcal

## Data Quality
- ✅ Curated list of popular Ukrainian foods
- ✅ Accurate nutritional breakdown
- ✅ Proper categorization
- ✅ Ready for application use
