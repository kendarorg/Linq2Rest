# Welcome #

I wrote a quick [tutorial](http://www.reimers.dk/tutorials/using-linq2rest) on how to get started with Linq2Rest on my site.

In short, in your web application, if 'source' is an IEnumerable, then it can be filtered using:

```
#!c#

var filteredSource = source.Filter(Request.Params);
```

To set up a client read Peter Goodman's [post](http://blog.petegoo.com/index.php/2012/03/11/creating-a-net-queryable-client-for-asp-net-web-api-odata-services/) on how to use Linq2Rest as a client for a WebApi service.

Have fun!

[![Supported by ReSharper][imgurl]][linkurl]

[imgurl]: https://bitbucket.org/jjrdk/linq2rest/raw/c53b0ac9a2fc4c87567d46b5367d3d84899460b2/resharper.png
[linkurl]: http://www.jetbrains.com/resharper/