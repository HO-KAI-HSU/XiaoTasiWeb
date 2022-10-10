
$(function () {

    // 初始化設定搜尋開始時間 
    var startTime = getToday();
    $("#start_time").val(startTime);
    $('#start_time').attr('min', startTime);

    // loading 頁面時 query API  
    foreignTripList(startTime);

    // 按鈕 query API  
    $(".one_day_search_btn").on("click", function () {
        var sTime = $("#start_time").val();
        var location = $("#location").val();
        foreignTripList(sTime, location);
    });
});

// 取得國外旅遊詳情 API 模塊 
function foreignTripList(_sDate = "", _location = "") {
    $.post('/Trip/GetTravelListForMember', { page: 1, limit: 100, travelType: 5, searchDate: _sDate, searchString: _location }).done(function (tripList) {
        var item = "";
        $.each(tripList.travelList, function (i, trip) {
            if (i == 0 && i % 3 == 0) {
                // 第一次
                var row = 0;
                item = `<ul class="news_content clearfix mt0">`;
            } else if (i != 0 && i % 3 == 0) {
                // 第N次
                row++;
                item += `</ul>`;
                item += `<ul class="news_content clearfix">`;
            }
            var picPath = ((trip.travelPicPath == "" || trip.travelPicPath == " ") ? "../images/trip/multipleday_trip_photo2.png" : trip.travelPicPath);
            item += `<li>
                    <a href="#" id="tripInfo"><img class="trip" src="${picPath}" alt="台北近郊一日遊" title="台北近郊一日遊"></a>
                    <input type="hidden" id="travelCode" value="${trip.travelCode}">
                    <span class="date">${trip.travelFdate}</span>
                    <h4>${trip.travelTraditionalTitle}</h4>
                    <p>出發日期:<span>${trip.startDate}</span></p>
                    <p class="travel_fee">旅費:<span class="dollars">${trip.cost}</span>元起</p>
                    </li>`;
            if (i == (tripList.length - 1)) {
                item += `</ul>`;
            }
        });
        $('#tab_4').html(item);
    });
}

// 取得今日日期  
function getToday() {
    var today = new Date();
    var startTime = today.getFullYear() + '-' + ('0' + (today.getMonth() + 1)).slice(-2) + '-' + ('0' + today.getDate()).slice(-2);
    return startTime;
}