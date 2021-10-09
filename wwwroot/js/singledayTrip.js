  
$(function () {

    // 初始化設定搜尋開始時間 
    var startTime = getToday();
    $("#start_time").val(startTime);

    // loading 頁面時 query API 
    singledayTripList(startTime);

    // 按鈕 query API 
    $(".one_day_search_btn").on("click", function () {
        var sTime = $("#start_time").val();
        var location = $("#location").val();
        singledayTripList(sTime, location);
    });
});

// 取得一日旅遊 API 模塊
function singledayTripList(_date = null, _location = "") {
    var today = new Date();
    var date = _date == null ? today.getFullYear() + '-' + ('0' + (today.getMonth() + 1)).slice(-2) + '-' + ('0' + today.getDate()).slice(-2) : _date;
    var location = _location;
    $.post('/Trip/GetSingleTravelListForMember', { page: 1, limit: 100, searchDate: date, searchString: location}).done(function (tripList) {
        var item = "";
        $.each(tripList.travelList, function (i, trip) {
            var travelPicItem = `<ul class="place_detail clearfix">`;
            $.each(trip.dateTravelPicList[0].travelPicList, function (i, travelPicInfo) {
                travelPicItem += `<li><span class="place_s_title">${travelPicInfo.travelPicTraditionalTitle}:</span>${travelPicInfo.travelPicTraditionalIntro}</li>`;
            });
            travelPicItem += `</ul>`;
            item += `<li>
                    <img src="${trip.travelPicPath}" alt="台北近郊一日遊" title="台北近郊一日遊">
                    <div class="news_content_right">
                        <h4>${trip.travelTraditionalTitle}</h4>
                        <p class="one_day_time"><span class="news_s_title">出發日期:</span><span class="time">${trip.travelFdate}</span>(<span class="day">星期一</span>)</p>
                        <p class="one_day_brief"><span class="news_s_title">行程簡述:</span>台版紐西蘭之美譽，絕美海景、馬兒、羊群與你相伴。</p>`;
            item += travelPicItem;
            item += `
                        <p class="travel_fee">行程旅費:<span class="dollars">${trip.cost}</span>元起</p>
                        <a class="one_day_booking_btn" href="#">立即報名</a>
                   </div>
                </li>`;
        });
        $("#start_time").val(date);
        $('.news_content').html(item);
    });
}

// 取得今日日期  
function getToday() {
    var today = new Date();
    var startTime = today.getFullYear() + '-' + ('0' + (today.getMonth() + 1)).slice(-2) + '-' + ('0' + today.getDate()).slice(-2);
    return startTime;
}