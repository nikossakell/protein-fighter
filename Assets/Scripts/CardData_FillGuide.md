# CardData Inspector Filling Guide

# Fill each ScriptableObject in Assets/Resources/Cards/ with these values.

# Asset name convention: Card\_<id>.asset

# --- CONFIRMED FROM DEMO CARD IMAGES ---

id: avocado
displayNameEl: ΑΒΟΚΑΝΤΟ
rank: Silver # adjust to your 30-card rank distribution
artwork: avocado_card.png
proteins: 5
carbs: 15
fats: 25
vitamins: 10
omega3: 10
fiber: 25

# Verify: AttackTotal = 5+15+25 = 45 ✓ DefenseTotal = 10+10+25 = 45 ✓

nutritionFactEl: "Το αβοκάντο είναι πλούσιο σε μονοακόρεστα λιπαρά και φυτικές ίνες, που συμβάλλουν στη μείωση της κακής χοληστερόλης και στην καλή λειτουργία του πεπτικού."

---

id: banana
displayNameEl: ΜΠΑΝΑΝΑ
rank: Bronze
artwork: banana_card.png
proteins: 15
carbs: 25
fats: 5
vitamins: 15
omega3: 5
fiber: 15

# AttackTotal = 15+25+5 = 45 ✓ DefenseTotal = 15+5+15 = 35 ✓

nutritionFactEl: "Η μπανάνα προσφέρει γρήγορη ενέργεια χάρη στους φυσικούς υδατάνθρακές της και είναι εξαιρετική πηγή καλίου για τη μυϊκή λειτουργία."

---

id: chicken
displayNameEl: ΚΟΤΟΠΟΥΛΟ
rank: Gold
artwork: chicken_card.png
proteins: 60
carbs: 0
fats: 18
vitamins: 10
omega3: 5
fiber: 0

# AttackTotal = 60+0+18 = 78 ✓ DefenseTotal = 10+5+0 = 15 ✓

nutritionFactEl: "Το κοτόπουλο (χωρίς δέρμα) είναι άπαχη πηγή υψηλής ποιότητας πρωτεΐνης, απαραίτητης για τη μυϊκή ανάπτυξη και την επιδιόρθωση ιστών."

---

id: salmon
displayNameEl: ΣΟΛΟΜΟΣ
rank: Gold
artwork: salmon_card.png
proteins: 50
carbs: 0
fats: 25
vitamins: 20
omega3: 50
fiber: 5

# AttackTotal = 50+0+25 = 75 ✓ DefenseTotal = 20+50+5 = 75 ✓

nutritionFactEl: "Ο σολομός είναι εξαιρετική πηγή Ω3 λιπαρών οξέων που μειώνουν τη φλεγμονή, προστατεύουν το καρδιαγγειακό σύστημα και υποστηρίζουν τη λειτουργία του εγκεφάλου."

---

# --- TEMPLATE FOR REMAINING 26 CARDS ---

# Copy one block per card. Replace placeholders.

id: <food_name_en_lowercase>
displayNameEl: <ΟΝΟΜΑ ΣΤΑ ΕΛΛΗΝΙΚΑ>
rank: Bronze | Silver | Gold | Platinum
artwork: <filename>.png
proteins: <int>
carbs: <int>
fats: <int>

# Verify: proteins + carbs + fats = ΕΠ printed on your PNG

vitamins: <int>
omega3: <int>
fiber: <int>

# Verify: vitamins + omega3 + fiber = ΑΜ printed on your PNG

nutritionFactEl: "<2–3 sentences in Greek about the food's key nutritional value>"

---

# RANK DISTRIBUTION TARGETS (for 30 cards):

# Bronze: ~17 cards (55% drop rate)

# Silver: ~ 9 cards (30%)

# Gold: ~ 4 cards (12%)

# Platinum: ~ 2 cards (3%) ← make these the most nutritionally complete foods

# PLATINUM CANDIDATES (highest combined ΕΠ+ΑΜ, educationally exemplary):

# salmon (75+75=150)
