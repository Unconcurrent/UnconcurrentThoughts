module internal Articles

#if INTERACTIVE
#load "Authors.fsx"
#load "ArticleTools.fsx"
#load "Articles/ArticleTypeProvider.fsx"
#load "Articles/ArticleSoloDBvsLiteDB.fsx"
#endif

let internal allArticles = [
    ArticleSoloDBvsLiteDB.get()
    ArticleTypeProvider.get()
]