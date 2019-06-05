# MovieRating

## 问题  


由于之前的计算器已经使用过[Material Design In XAML](http://materialdesigninxaml.net/)了，整体控件的使用上顺畅不少，
然后使用[Dragablz](https://dragablz.net/)与上面的MD主题结合，做出了几个Tab。

之后就是使用WindowStyle=None,AllowTransparency=True的方法进行无边框窗口设计，并自行重写标题栏，
但是最大化、最小化、还原的动画效果没有了，而且程序运行起来非常生涩，
于是按照《[[WPF]使用WindowChrome自定义Window Style](https://www.cnblogs.com/dino623/p/CustomWindowStyle.html)》一文中的教程转到使用WindowChrome来自定义窗口。

改好之后发现窗口最大化之后总是会向左上偏移若干像素，然后按照《[WPF在自定义窗口样式时，如何处理最大化时边框溢出屏幕外侧的问题？](https://social.msdn.microsoft.com/Forums/zh-CN/16725ba8-6cc5-4cb4-9a77-b30e20a8f169/wpf223123325823450200413138321475266792433526102652922291420309?forum=wpfzhchs)》的思路，
把RootLayout那个Grid的最大宽高Bind了Workarea的宽高，解决了问题。

参照《[带有自定义镶边的WPF窗口在右侧和底部有不需要的轮廓](https://codeday.me/bug/20190304/758115.html)》解决了最大化还原二选一的问题。

这样改完之后头疼的由于StackPanel高度无限导致的滚动条高度设置必须写死也解决了。

在尝试使用Entity Framework时，还在用老脑筋去思考，弄了半天联接两个表进行查询，Jion方法鼓捣了半天，Include方法也鼓捣了半天，后来才发现EF里的实体定义的时候已经包含了所有相关的内容，就是说已经通过主外键连接好了，不需要自己再折腾了。

做到影片类别的时候发现设计的数据库挺蠢的，每一个类别都用一个Boolean表示，在Binding到Converter的时候就得Multi Binding十九个变量，太麻烦了，后来处理了一下数据，将类别信息转换为十九位的01字串，处理起来就好多了。

https://stackoverflow.com/questions/3520713/programmatically-raise-a-command 

https://www.codercto.com/a/52275.html

 [EntityFramework之异步、事务及性能优化（九）](https://www.cnblogs.com/CreateMyself/p/4787856.html)

 https://www.chengxulvtu.com/entityframework-nested-query/

 https://www.cnblogs.com/zhchbin/archive/2012/03/06/2381693.html

 https://stackoverflow.com/a/34400406

 随机数重复，范围较小