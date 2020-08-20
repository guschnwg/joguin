deploy:
	git add build -f && git commit -m "Deploy" && git push origin `git subtree split --prefix build master`:gh-pages --force && git reset HEAD~1
