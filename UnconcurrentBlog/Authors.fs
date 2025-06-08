module private Authors

type Author = {
    Name: string
    Link: string option
}

let Unconcurrent = { Name = "Unconcurrent"; Link = None }