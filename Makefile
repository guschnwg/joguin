deploy:
	git add build && git commit -m "Deploy" && git push origin `git subtree split --prefix build master`:gh-pages --force
