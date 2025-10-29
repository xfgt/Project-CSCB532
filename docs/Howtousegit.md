# Как да ползваме git & github
_направено **без** gpt_

## Регистрация за потребител
```bash
git config --global user.name "Your Name"
git config --global user.email "you@example.com"

# Това ни позволява да 
# pull-ваме (изтегляме промени от github)
# и push-ваме (добавяме промени към github)  
```

## Създаване на нов проект

1. Създава се репото първо в github.com
2. Инициализира се локално на компютъра, като:
```bash 
mkdir myapp && cd myapp       # правиш папка и влизаш в нея
git init                      # инициализираш репото (създава се важната .git скрита папка)
echo "# My App" > README.md   # Правиш едно readme.md, като в него ще пише това което е в кавичките след 'echo'


git status                    # виж кои файлове са променени (доста полезно)
                              # ако са в червено не са stage-нати (трябва да се stage-нат с git add)
                              # ако са в зелено са готови за commit (излитане)


git add .                     # stage-ва променените файлове и ги подготвя за commit

                              # точката  е ТУКАШНАТА дериктория
                              # тълкува се като (добави всичко в тази папка, което е променено)
                              
# =========ВАЖНО=================
# АКО ИМАМЕ .gitignore ФАЙЛ В СЪЩАТА ПАПКА, ТО ФАЙЛОВЕТЕ КОИТО СА СПОМЕНАТИ ВЪТРЕ,
# НЕ ПРИСЪСТВАТ В "git status" 


git commit -m "chore: initial commit" # Създава комит със съобщение (-m)
git log # показва историята на комитите (доста полезно)
git branch -M main                    # Това се прави веднъж (сменя името на клона {branch-а} от default-ния "master" на "main"

# Добавяме remote репото което се води под променливата 'origin', характерна за git системата
# origin <- "that link"
git remote add origin https://github.com/USERNAME/REPONAME.git

# Пускаме пакета пакета с промени (комита) към remote репото
git push -u origin main

# Това действие може да изисква token (прави се тук: https://github.com/settings/tokens)
```


## Среди на проекта
* Локална
* Remote

>промените в локалната среда (папката на компа)
трябва да бъдат упоменати с комит, одобрени от собственика на репото,
и актуализирани (merge-нати) към главния клон (branch): "main"



## За тези, които ще правят промени

### Изтегляне на репото
```bash
# предварително трябва да се намираш в папка, която ще ти е удобна за работа
# старта на репото е там, където се намира скритата папка .git

# Windows
mkdir C:\Users\USERNAME\Desktop\github-nqkakuv-proekt
cd C:\Users\USERNAME\Desktop\github-nqkakuv-proekt

# Linux
mkdir ~/Desktop/github-nqkakuv-proekt
cd ~/Desktop/github-nqkakuv-proekt

# изтегляме цялото репо на нашия комп (локално)
git clone https://github.com/USERNAME/REPONAME  
# всичко което виждаше в github сега е в папката "github-nqkakuv-proekt"
```

### Добавяне на промени по безопасен начин

В началото репото е само на един клон (branch) -> "main".
```bash
git branch                  # виж съществуващите бранчове
git checkout <branch-name>  # смени да работиш на друг клон
```
**Лоша практика е да се правят директно промени от всички в main**, защото не се уведомяват всички работещи по проекта, когато са настъпили промени.

Подход:

```bash
git checkout -c <branch-name>   # създаване + сменяне на нов клон

git push origin <branch-name>   # пушваме новия клон, за да се появи в сайта 

# оттук вече си на друг клон различен от main и може спокойно да правиш
# промени, които после да преценим да MERGE-нем към main клона
```

Обичаен сценарии **за започване**:
```bash
mkdir asdf && cd asdf
git clone <repo-link>
git checkout -c <new-branch-name>

# някаква съществена промяна (нов файл)
touch asdf.txt

git add asdf.txt
git commit -m "created new file asdf.txt"
git log # проверка на комита дали е регистриран в историята
git push origin <new-branch-name> # Пускаме промените от нашия клон към remote-a (origin) -> https://github.com/USERNAME/REPONAME
# това прави Pull request към притежателя: ако няма конфликти той одобрява и MERGE-ва промените към главния клон main
```

# СИНХРОНИЗАЦИЯ С REMOTE

```bash
git pull origin main # взимаме промените от main
```