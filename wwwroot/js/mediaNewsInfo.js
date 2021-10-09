
$(function () {
    // loading 頁面時 query API  
    mediaNewsInfo();
});


// 取得最新消息
function mediaNewsInfo() {
    var newsInfo = JSON.parse(localStorage.getItem('mediaNewsInfo'));
    var title = `${newsInfo.newsTitle}`;
    var imgUrl = newsInfo.url;
    var content = `<p>${newsInfo.newsContent}</p>`;

    $(".news_photo").attr('src', imgUrl);
    $(".news_page_title").html(title);
    $(".mt32").html(content);
}