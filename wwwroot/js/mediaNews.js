  
$(function () {

    // 初始化設定搜尋開始時間 
    var startTime = getToday();
    $("#start_time").val(startTime);

    // loading 頁面時 query API  
    newsList();

    $(document).on("click", "li", function () {
        console.log("news_slide");
        var imgUrl = $(this).find("img").attr('src');
        var newsDate = $(this).children(".newsDate").text();
        var newsTitle = $(this).children(".newsTitle").text();
        var newsContent = $(this).children("p.newsContent").text();
        console.log(newsDate);
        console.log(newsTitle);
        console.log(newsContent);
        var newsInfo = {
            url: imgUrl,
            newsDate: newsDate,
            newsTitle: newsTitle,
            newsContent: newsContent
        };
        localStorage.setItem("mediaNewsInfo", JSON.stringify(newsInfo));
        window.location.href = '/Home/MediaNewsInfo';
    });
});


// 取得最新媒體消息 API 模塊 
function newsList(_sDate = "") {
    console.log(_sDate);
    $.post('/News/getMediaNewsList', { page: 1, limit: 50, searchDate: _sDate }).done(function (newslist) {
        var item = `<h1>傳媒報導<span class="orange_title">特集</span></h1>`;
        $.each(newslist.mediaNewsList, function (i, news) {
            if (i % 3 == 0) {
                // 第一次 
                item += `<ul class="news_content clearfix">`;
            }
            item += `
                <li>
                    <img class="news_photo" src="${news.mediaNewsPicPath}" alt="小蔡旅遊-國泰航空建立夥伴關係" title="小蔡旅遊-國泰航空建立夥伴關係">
                    <span class="date">${news.date}</span>
                    <h4 class="newsTitle">${news.mediaNewsTraditionalTitle}</h4>
                    <p class="newsContent">${news.mediaNewsTraditionalContent}</p>
                </li>`;
            if (i % 3 == 2) {
                // 第一排尾部 
                item += `</ul>`;
            }
        });
        $(".mt70").append(item);
    });
}

// 取得今日日期  
function getToday() {
    var today = new Date();
    var startTime = today.getFullYear() + '-' + ('0' + (today.getMonth() + 1)).slice(-2) + '-' + ('0' + today.getDate()).slice(-2);
    return startTime;
}