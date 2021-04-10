  
$(function () {
    $.post('/Trip/GetTravelListForMember', { page: 1, limit: 100, travelType: 5}).done(function (tripList) {
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

            item += `<li>
                    <a href="#" id="tripInfo"><img src="/images/multipleday_trip_photo2.png" alt="台北近郊一日遊" title="台北近郊一日遊"></a>
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
        $('#tab_4').append(item);
	});
});