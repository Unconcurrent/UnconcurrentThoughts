module internal Articles

#if INTERACTIVE
#load "Authors.fsx"
#load "ArticleTools.fsx"
#load "ArticlePreprocessor.fsx"
#load "Articles/ArticleCommonUndergroundII.fsx"
#load "Articles/ArticleCommonUnderground.fsx"
#load "Articles/ArticleTypeProvider.fsx"
#load "Articles/ArticleSoloDB120.fsx"
#load "Articles/ArticleSoloDB110.fsx"
#load "Articles/ArticleSoloDBvsLiteDB.fsx"
#load "Articles/ArticleSoloDBOrg.fsx"
#load "Articles/ArticleSoloDB100.fsx"
#endif

let internal allArticles = [
    ArticleCommonUndergroundII.get()
    ArticleCommonUnderground.get()
    ArticleSoloDB120.get()
    ArticleSoloDB110.get()
    ArticleSoloDB100.get()
    ArticleSoloDBOrg.get()
    ArticleSoloDBvsLiteDB.get()
    ArticleTypeProvider.get()
]

let internal staticFiles = [
    yield! ArticleCommonUnderground.staticFiles
]
