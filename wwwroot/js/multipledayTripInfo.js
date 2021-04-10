  
$(function () {

    // localstorage 取得旅遊編碼 
    var travelCode = localStorage.getItem('travelCode');

    // loading 頁面時 query API
    multipledayTripInfo(travelCode);

    // 按鈕更新旅遊統計
    $(".travel_step_code_btn").on("click", function () {
        var travelStepCode = $("#travel_step_code").val();
        updateTripStatisticInfo(travelStepCode);
    });
});


// 取得多日旅遊詳情 API 模塊 
function multipledayTripInfo(_travelCode = "", _travelStepCode = "") {
    $.post('/Trip/GetTravelInfoForMember', { travelCode: _travelCode, travelStepCode: _travelStepCode}).done(function (tripInfo) {
        console.log(tripInfo);
        var dateTravelItem = "";          // 每日旅遊資訊
        var dateReducedTravelItem = "";   // 精簡模式資訊
        var dateTravelCostItem = "";      // 費用資訊 
        var videoSliderItem = "";         // 整理影片資訊 
        var travelTitleItem = "";         // 整理旅遊標題資訊 
        var travelStatisticsItem = "";    // 整理統計資訊
        var travelContentItem = "";       // 整理旅遊內容資訊
        var travelTitle = tripInfo.travelInfo.travelTitle;
        var travelContent = tripInfo.travelInfo.travelContent;

        // 整理影片資訊 e
        videoSliderItem += `
            <div class="video_slider">
                <div class="slick-slide">
                    <img class="multiple_day_video_photo" src="/images/multipleday_video.png" alt="行程影片" title="行程影片">
                </div>
                <div class="slick-slide">
                    <img class="multiple_day_video_photo" src="/images/multipleday_video.png" alt="行程影片" title="行程影片">
                </div>
                <div class="slick-slide">
                    <img class="multiple_day_video_photo" src="/images/multipleday_video.png" alt="行程影片" title="行程影片">
                </div>
            </div>
        `;

        // 整理旅遊標題資訊
        travelTitleItem += `<h3>${travelTitle}<span class="video_mark">世界級的風景饗宴</span></h3>`;

        // 整理旅遊內容資訊
        travelContentItem += `${travelContent}`;

        // 整理旅遊精簡模式資訊
        dateReducedTravelItem += `
                    <tr>
                        <th class="order_date">天數</th>
                        <th class="place_food_hotel">旅遊行程/景點</th>
                        <th class="place_food_hotel">餐食</th>
                        <th class="place_food_hotel">旅館</th>
                    </tr>
        `;

        // 整理旅遊費用數說明資訊 
        dateTravelCostItem += `
                    <div class="tabs_fee_text">
                        <h3>費用說明</h3>
                        <p>1.車資：${tripInfo.costInfo.transportationInfo}</p>
                        <p>2.餐食：${tripInfo.costInfo.eatInfo}</p>
                        <p>3.住宿：${tripInfo.costInfo.liveInfo}</p>
                        <p>4.活動：${tripInfo.costInfo.actionInfo}</p>
                        <p>5.貼心：${tripInfo.costInfo.insuranceInfo}</p>
                        <p>6.保險：${tripInfo.costInfo.nearInfo}</p>
                        <p class="tabs_fee_text_notice">＊旅客未滿15歲或70歲以上，依法限制最高【意外死殘保額新臺幣20萬元、意外醫療保額新臺幣20萬 (實支實付)】</p>
                    </div>
                    <div class="tabs_fee_text">
                        <h3>費用不包含</h3>
                        <p>1.旅行業為服務業，所以小費一直是領隊和司機的主要收入之一，世界各國皆如此，台灣也不例外。所以小費一直是領隊和司機的主要收入之一主要收入之一以下為建議小費，敬請參考：每人每天100元，二天共200元。</p>
                        <p>2.個人因素所產生之消費，如飲料、酒類、私人購物費…等。</p>
                        <p>3.個人旅遊平安保險，依規定旅客若有個別需求，得自行投保旅行平安保險。</p>
                        <p>4.本行程表上未註明之各項開銷，建議、自費或自由行程所衍生之任何費用。建議、自費或自由行程所衍生之任何費用。建議、自費或自由行程所衍生費用。</p>
                    </div>
        `;


        $.each(tripInfo.dateTravelPicList, function (dateIndex, dateTravelPicInfo) {
            dateReducedTravelItem += `
                    <tr>
                        <td class="water_blue">第${dateIndex + 1}天</td>
            `;
            dateTravelItem += `
                    <div class="timeline_travel">
                    <h3>第${dateIndex + 1}天</h3>
                    <div class="timeline_box clearfix">`;
            var dateTravelPicItem = "";
            var viewPointStr = "";
            var mealStr = `早餐：${dateTravelPicInfo.mealInfo.breakfast}<br />午餐：${dateTravelPicInfo.mealInfo.lunch}<br />晚餐：${dateTravelPicInfo.mealInfo.dinner}`;
            var hotelStr = `${dateTravelPicInfo.hotel}`;
            $.each(dateTravelPicInfo.travelPicList, function (picIndex, travelPicInfo) {
                if (picIndex % 2 == 0) {
                    dateTravelPicItem += `<div class="timeline_detail">`;
                } else {
                    dateTravelPicItem += `<div class="timeline_detail">`;
                }
                dateTravelPicItem += `
                    <div class="timeline_content">
                        <img src="/images/travel_photo1.jpg" alt="旅遊景點照片" title="鼻頭角遊憩區">
                        <h4>${travelPicInfo.travelPicTraditionalTitle}</h4>
                        <p>${travelPicInfo.travelPicTraditionalIntro}</p>
                    </div></div>`;

                if (picIndex == (dateTravelPicInfo.travelPicList.length - 1)) {
                    viewPointStr += `${travelPicInfo.travelPicTraditionalTitle}`;
                } else {
                    viewPointStr += `${travelPicInfo.travelPicTraditionalTitle}<br />`;
                }
            });

            dateReducedTravelItem += `
                        <td class="td_bottom">${viewPointStr}</td>
                        <td class="td_bottom">${mealStr}</td>
                        <td class="td_bottom">${hotelStr}</td>
                    </tr>
            `;

            dateTravelItem += dateTravelPicItem;
            dateTravelItem += `</div></div>`;
        });

        // localstorage 存儲旅遊統計 
        localStorage.setItem("travelStatisticsList", JSON.stringify(tripInfo.travelStatisticList));

        // 整理旅遊統計  
        $.each(tripInfo.travelStatisticList, function (dateIndex, travelStatisticInfo) {
            if (travelStatisticInfo.travelStepSelectFlag == 1) {
                travelStatisticsItem += `
                    <tr><th>去程時間</th><td>${travelStatisticInfo.startDate}</td></tr>
                    <tr><th>回程時間</th><td>${travelStatisticInfo.endDate}</td></tr>
                    <tr><th>天數</th><td>${travelStatisticInfo.dayNum}天</td></tr>
                    <tr><th>團數</th><td>${travelStatisticInfo.travelStep}</td></tr>
                    <tr><th>可賣</th><td>${travelStatisticInfo.remainSeatNum}</td></tr>
                    <tr><th>團位</th><td>${travelStatisticInfo.travelNum}</td></tr>
                    <tr><th>旅費</th><td><span class="dollars">${travelStatisticInfo.travelCost}</span>元起</td></tr>
                `;
            }
        });

        $(".video_slider").html(videoSliderItem);
        $(".travel_title_text").html(travelTitleItem);
        $(".travel_info_post_title").html(travelTitle);
        $(".travel_info_post_table").html(travelStatisticsItem);
        $(".travel_content_text").html(travelContentItem);
        $(".timeline_travel_content").html(dateTravelItem);
        $(".order_table").html(dateReducedTravelItem);
        $("#tab_2").html(dateTravelCostItem);
    });
}

// 更新多日旅遊統計模塊
function updateTripStatisticInfo(travelStepCode = "") {
    // localstorage 取得旅遊統計 
    var travelStatisticsList = JSON.parse(localStorage.getItem('travelStatisticsList'));
    console.log(travelStatisticsList);
    var updateTravelStatisticsItem = "";
    $.each(travelStatisticsList, function (dateIndex, travelStatisticInfo) {
        if (travelStatisticInfo.travelStep == travelStepCode) {
            updateTravelStatisticsItem += `
                <tr><th>去程時間</th><td>${travelStatisticInfo.startDate}</td></tr>
                <tr><th>回程時間</th><td>${travelStatisticInfo.endDate}</td></tr>
                <tr><th>天數</th><td>${travelStatisticInfo.dayNum}天</td></tr>
                <tr><th>團數</th><td>${travelStatisticInfo.travelStep}</td></tr>
                <tr><th>可賣</th><td>${travelStatisticInfo.remainSeatNum}</td></tr>
                <tr><th>團位</th><td>${travelStatisticInfo.travelNum}</td></tr>
                <tr><th>旅費</th><td><span class="dollars">${travelStatisticInfo.travelCost}</span>元起</td></tr>
            `;
        }
    });
    $(".travel_info_post_table").html(updateTravelStatisticsItem);
}
