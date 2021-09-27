  
$(function () {

    // 初始化設定搜尋開始時間 
    var startTime = getToday();
    $("#start_time").val(startTime);

    // loading 頁面時 query API  
    allTripList(startTime, "", 2, "tab_1");
    newsList();
    bannerList();
    $("#tab_1").show();

    $("ul.two_day_tabs").find("li").on("click", function () {
        let tab = $(this).find("a").attr("href");
        $(".tab_content[class!='tab_content_1']").hide();
        $(this).addClass("two_day_current").siblings().removeClass("two_day_current");
        $(tab).fadeIn(300);
        switch (tab) {
            case "#tab_1":
                allTripList(startTime, "", 2, "tab_1");
                $("#tab_1").show();
                break;
            case "#tab_2":
                allTripList(startTime, "", 3, "tab_2");
                $("#tab_2").show();
                break;
            case "#tab_3":
                allTripList(startTime, "", 4, "tab_3");
                $("#tab_3").show();
                break;
            case "#tab_4":
                allTripList(startTime, "", 5, "tab_4");
                $("#tab_4").show();
                break;
        }
        return false;
    });
});

// 取得所有旅遊 API 模塊 
function allTripList(_sDate = "", _location = "", _travelType = 2, _tabName = "tab_1") {
    console.log(_sDate);
    var tripItem = "";
    $.post('/Trip/GetTravelListForMember', { page: 1, limit: 8, travelType: _travelType, searchDate: _sDate, searchString: _location }).done(function (tripList) {
        $.each(tripList.travelList, function (i, trip) {
            if (i == 0 && i % 4 == 0) {
                // 第一次
                var row = 0;
                tripItem = `<ul class="index_two_day_trip mobile_index_two_day_trip clearfix">`;
            } else if (i != 0 && i % 4 == 0) {
                // 第N次
                row++;
                tripItem += `</ul>`;
                tripItem += `<ul class="index_two_day_trip mobile_index_two_day_trip clearfix mt22">`;
            }

            tripItem += `<li>
                        <img src="/images/multipleday_trip_photo2.png" alt="台南文化古都三日遊" title="台南文化古都三日遊">
                        <input type="hidden" id="travelCode" value="${trip.travelCode}">
                        <span class="date">${trip.travelFdate}</span>
                        <h4>${trip.travelTraditionalTitle}</h4>
                        <p>出發日期:<span>${trip.startDate}</span></p>
                        <p class="travel_fee">旅費:<span class="dollars">${trip.cost}</span>元起</p>
                    </li>`;
            if (i == (tripList.length - 1)) {
                tripItem += `</ul>`;
            }
        });
        console.log(tripItem);
        $('#' + _tabName).html(tripItem);
    });
}

// 取得最新消息 API 模塊 
function newsList(_sDate = "") {
    console.log(_sDate);
    $.post('/News/getLatestNewsList', { page: 1, limit: 50, searchDate: _sDate }).done(function (newslist) {
        var newsItem = "";
        $.each(newslist.lateNewsList, function (i, news) {
            newsItem += `
                <li class="news_slide">
                    <img src="/images/index_trip_photo1.png" alt="台北近郊二日遊-沙崙海水浴場" title="台北近郊二日遊-沙崙海水浴場">
                    <span class="date">${news.date}</span>
                    <h4>${news.newsTraditionalTitle}</h4>
                    <p>${news.newsTraditionalContent}</p>
                </li>`;
        });
        $("ul.news_slider").append(newsItem);
        $('.slick-slider').slick('refresh');
    });
}

// 取得banner圖 API 模塊
function bannerList() {
    var bannerItem = `
        <div class="slick-slide">
            <img src="/images/index_banner1.png" alt="首頁banner">
        </div>
        <div class="slick-slide">
            <img src="/images/index_banner2.png" alt="首頁banner">
        </div>
        <div class="slick-slide">
            <img src="/images/index_banner3.png" alt="首頁banner">
        </div>`;
    $(".index_slider").append(bannerItem);
    $('.slick-slider').slick('refresh');
}

// 取得今日日期  
function getToday() {
    var today = new Date();
    var startTime = today.getFullYear() + '-' + ('0' + (today.getMonth() + 1)).slice(-2) + '-' + ('0' + today.getDate()).slice(-2);
    return startTime;
}