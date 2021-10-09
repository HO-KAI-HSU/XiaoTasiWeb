  
$(function () {
    // loading 頁面時 query API  
    newsInfo();
});


// 取得最新消息
function newsInfo() {
    var newsInfo = JSON.parse(localStorage.getItem('newsInfo'));
    var title = `${newsInfo.newsTitle}`;
    var imgUrl = newsInfo.url;
    var content = `<p>${newsInfo.newsContent}</p>`;

    $(".news_photo").attr('src', imgUrl);
    $(".news_page_title").html(title);
    $(".mt32").html(content);
}