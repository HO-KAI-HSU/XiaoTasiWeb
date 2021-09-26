  
$(function () {

    // 初始化設定搜尋開始時間 
    var startTime = getToday();
    $("#start_time").val(startTime);

    // loading 頁面時 query API  
    newsList();
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
                    <img class="ticket_icon" src="/images/ticket_icon.png" alt="車票ICON">
                    <img class="news_photo" src="/images/news_photo2.png" alt="小蔡旅遊-國泰航空建立夥伴關係" title="小蔡旅遊-國泰航空建立夥伴關係">
                    <span class="date">${news.date}</span>
                    <h4>${news.mediaNewsTraditionalTitle}</h4>
                    <p>${news.mediaNewsTraditionalContent}</p>
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