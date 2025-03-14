# service-bank
git init

touch index.html
git add .
git commit -m 'create page'

touch style.css
git add .
git commit -m 'create stylesheet'

touch script.js
git add .
git commit -m 'create function'

# Cherry Pick

git checkout branch derivada de dev
abriu o MR de branch baseada em MAIN
fazer git cherry-pick dos commit mais antigos pros mais novos
a cada fim de cherry executa git commit
no final git push na branch derivada de dev

# REBASE - SQAUSH COMMIT

git rebase -i HEAD <commit_id>

press 'i' to change files
but first of all was pick

press 'esc' to finish file change
press ':' to execute new command
press 'wq' to write changes and close file
press 'enter' to resume
