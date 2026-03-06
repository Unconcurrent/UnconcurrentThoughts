module internal Articles

#if INTERACTIVE
#load "Authors.fsx"
#load "ArticleTools.fsx"
#load "Articles/ArticleTypeProvider.fsx"
#load "Articles/ArticleSoloDB110.fsx"
#load "Articles/ArticleSoloDBvsLiteDB.fsx"
#load "Articles/ArticleSoloDBOrg.fsx"
#load "Articles/ArticleSoloDB100.fsx"
#endif

let internal allArticles = [
    ArticleSoloDB110.get()
    ArticleSoloDB100.get()
    ArticleSoloDBOrg.get()
    ArticleSoloDBvsLiteDB.get()
    ArticleTypeProvider.get()
]
