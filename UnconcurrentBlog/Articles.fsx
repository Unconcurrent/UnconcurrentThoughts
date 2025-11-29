module internal Articles

#if INTERACTIVE
#load "Authors.fsx"
#load "ArticleTools.fsx"
#load "Articles/ArticleTypeProvider.fsx"
#load "Articles/ArticleSoloDBvsLiteDB.fsx"
#load "Articles/ArticleSoloDBOrg.fsx"
#endif

let internal allArticles = [
    ArticleSoloDBOrg.get()
    ArticleSoloDBvsLiteDB.get()
    ArticleTypeProvider.get()
]