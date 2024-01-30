$(function () {
    try {
        $('.daterange-basic').daterangepicker({
            showDropdowns: true
            , applyClass: 'bg-slate-600'
            , cancelClass: 'btn-default'
        });
    } catch (e) {

    }
    try {
        $('.daterange-weeknumbers').daterangepicker({
            showDropdowns: true
            , showWeekNumbers: true
            , applyClass: 'bg-slate-600'
            , cancelClass: 'btn-default'
        });
    } catch (e) {

    }
    try {
        $('.daterange-buttons').daterangepicker({
            showDropdowns: true
            , applyClass: 'btn-success'
            , cancelClass: 'btn-danger'
        });
    } catch (e) {

    }
    try {
        $('.daterange-time').daterangepicker({
            showDropdowns: true
            , timePicker: true
            , applyClass: 'bg-slate-600'
            , cancelClass: 'btn-default'
            , locale: {
                format: 'MM/DD/YYYY h:mm a'
            }
        });
    } catch (e) {

    }
    try {
        $('.daterange-left').daterangepicker({
            showDropdowns: true
            , opens: 'left'
            , applyClass: 'bg-slate-600'
            , cancelClass: 'btn-default'
        });
    } catch (e) {

    }
    //    $('.daterange-single').daterangepicker({
    //        showDropdowns: true,
    //        singleDatePicker: true,
    //        locale: {
    //            format: 'DD/MM/YYYY'
    //        }
    //    }); 
    try {
        $('.daterange-single').each(function (i, el) {
            $(el).daterangepicker({
                showDropdowns: true
                , singleDatePicker: true
                , autoUpdateInput: false
                , minDate: "01/01/1816"
                , maxDate: moment()
                , locale: {
                    format: 'DD/MM/YYYY'
                }
            }, function (chosen_date) {
                $(el).val(chosen_date.format('DD/MM/YYYY'));
            });
        });
    } catch (e) {

    }

    try {
        $('.daterange-datemenu').daterangepicker({
            showDropdowns: true
            , opens: "left"
            , applyClass: 'bg-slate-600'
            , cancelClass: 'btn-default'
        });
    } catch (e) {

    }
    try {
        $('.daterange-increments').daterangepicker({
            timePicker: true
            , opens: "left"
            , applyClass: 'bg-slate-600'
            , cancelClass: 'btn-default'
            , timePickerIncrement: 10
            , locale: {
                format: 'MM/DD/YYYY h:mm a'
            }
        });
    } catch (e) {

    }
    try {
        $('.daterange-locale').daterangepicker({
            showDropdowns: true
            , applyClass: 'bg-slate-600'
            , cancelClass: 'btn-default'
            , opens: "left"
            , ranges: {
                'Ð¡ÐµÐ³Ð¾Ð´Ð½Ñ': [moment(), moment()]
                , 'Ð’Ñ‡ÐµÑ€Ð°': [moment().subtract('days', 1), moment().subtract('days', 1)]
                , 'ÐŸÐ¾ÑÐ»ÐµÐ´Ð½Ð¸Ðµ 7 Ð´Ð½ÐµÐ¹': [moment().subtract('days', 6), moment()]
                , 'ÐŸÐ¾ÑÐ»ÐµÐ´Ð½Ð¸Ðµ 30 Ð´Ð½ÐµÐ¹': [moment().subtract('days', 29), moment()]
                , 'Ð­Ñ‚Ð¾Ñ‚ Ð¼ÐµÑÑÑ†': [moment().startOf('month'), moment().endOf('month')]
                , 'ÐŸÑ€Ð¾ÑˆÐµÐ´ÑˆÐ¸Ð¹ Ð¼ÐµÑÑÑ†': [moment().subtract('month', 1).startOf('month'), moment().subtract('month', 1).endOf('month')]
            }
            , locale: {
                applyLabel: 'Ð’Ð¿ÐµÑ€ÐµÐ´'
                , cancelLabel: 'ÐžÑ‚Ð¼ÐµÐ½Ð°'
                , startLabel: 'ÐÐ°Ñ‡Ð°Ð»ÑŒÐ½Ð°Ñ Ð´Ð°Ñ‚Ð°'
                , endLabel: 'ÐšÐ¾Ð½ÐµÑ‡Ð½Ð°Ñ Ð´Ð°Ñ‚Ð°'
                , customRangeLabel: 'Ð’Ñ‹Ð±Ñ€Ð°Ñ‚ÑŒ Ð´Ð°Ñ‚Ñƒ'
                , daysOfWeek: ['Ð’Ñ', 'ÐŸÐ½', 'Ð’Ñ‚', 'Ð¡Ñ€', 'Ð§Ñ‚', 'ÐŸÑ‚', 'Ð¡Ð±']
                , monthNames: ['Ð¯Ð½Ð²Ð°Ñ€ÑŒ', 'Ð¤ÐµÐ²Ñ€Ð°Ð»ÑŒ', 'ÐœÐ°Ñ€Ñ‚', 'ÐÐ¿Ñ€ÐµÐ»ÑŒ', 'ÐœÐ°Ð¹', 'Ð˜ÑŽÐ½ÑŒ', 'Ð˜ÑŽÐ»ÑŒ', 'ÐÐ²Ð³ÑƒÑÑ‚', 'Ð¡ÐµÐ½Ñ‚ÑÐ±Ñ€ÑŒ', 'ÐžÐºÑ‚ÑÐ±Ñ€ÑŒ', 'ÐÐ¾ÑÐ±Ñ€ÑŒ', 'Ð”ÐµÐºÐ°Ð±Ñ€ÑŒ']
                , firstDay: 1
            }
        });
    } catch (e) {

    }
    try {
        $('.daterange-predefined').daterangepicker({
            startDate: moment().subtract('days', 29)
            , endDate: moment()
            , minDate: '01/01/2014'
            , maxDate: '12/31/2016'
            , dateLimit: {
                days: 60
            }
            , ranges: {
                'Today': [moment(), moment()]
                , 'Yesterday': [moment().subtract('days', 1), moment().subtract('days', 1)]
                , 'Last 7 Days': [moment().subtract('days', 6), moment()]
                , 'Last 30 Days': [moment().subtract('days', 29), moment()]
                , 'This Month': [moment().startOf('month'), moment().endOf('month')]
                , 'Last Month': [moment().subtract('month', 1).startOf('month'), moment().subtract('month', 1).endOf('month')]
            }
            , opens: 'left'
            , applyClass: 'btn-small bg-slate'
            , cancelClass: 'btn-small btn-default'
        }, function (start, end) {
            $('.daterange-predefined span').html(start.format('MMMM D, YYYY') + ' &nbsp; - &nbsp; ' + end.format('MMMM D, YYYY'));
            $.jGrowl('Date range has been changed', {
                header: 'Update'
                , theme: 'bg-primary'
                , position: 'center'
                , life: 1500
            });
        });
    } catch (e) {

    }
    try {
        $('.daterange-predefined span').html(moment().subtract('days', 29).format('MMMM D, YYYY') + ' &nbsp; - &nbsp; ' + moment().format('MMMM D, YYYY'));
    } catch (e) {

    }
    try {
        $('.daterange-ranges').daterangepicker({
            startDate: moment().subtract('days', 29)
            , endDate: moment()
            , minDate: '01/01/2012'
            , maxDate: '12/31/2016'
            , dateLimit: {
                days: 60
            }
            , ranges: {
                'Today': [moment(), moment()]
                , 'Yesterday': [moment().subtract('days', 1), moment().subtract('days', 1)]
                , 'Last 7 Days': [moment().subtract('days', 6), moment()]
                , 'Last 30 Days': [moment().subtract('days', 29), moment()]
                , 'This Month': [moment().startOf('month'), moment().endOf('month')]
                , 'Last Month': [moment().subtract('month', 1).startOf('month'), moment().subtract('month', 1).endOf('month')]
            }
            , opens: 'left'
            , applyClass: 'btn-small bg-slate-600'
            , cancelClass: 'btn-small btn-default'
        }, function (start, end) {
            $('.daterange-ranges span').html(start.format('MMMM D, YYYY') + ' &nbsp; - &nbsp; ' + end.format('MMMM D, YYYY'));
        });
    } catch (e) {

    }
    try {
        $('.daterange-ranges span').html(moment().subtract('days', 29).format('MMMM D, YYYY') + ' &nbsp; - &nbsp; ' + moment().format('MMMM D, YYYY'));
    } catch (e) {

    }
    try {
        $('.pickadate').pickadate();

        $('.pickadate-strings').pickadate({
            weekdaysShort: ['Su', 'Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa']
            , showMonthsShort: true
        });
        $('.pickadate-buttons').pickadate({
            today: ''
            , close: ''
            , clear: 'Clear selection'
        });
        $('.pickadate-accessibility').pickadate({
            labelMonthNext: 'Go to the next month'
            , labelMonthPrev: 'Go to the previous month'
            , labelMonthSelect: 'Pick a month from the dropdown'
            , labelYearSelect: 'Pick a year from the dropdown'
            , selectMonths: true
            , selectYears: true
        });
        $('.pickadate-translated').pickadate({
            monthsFull: ['Janvier', 'FÃ©vrier', 'Mars', 'Avril', 'Mai', 'Juin', 'Juillet', 'AoÃ»t', 'Septembre', 'Octobre', 'Novembre', 'DÃ©cembre']
            , weekdaysShort: ['Dim', 'Lun', 'Mar', 'Mer', 'Jeu', 'Ven', 'Sam']
            , today: 'aujourd\'hui'
            , clear: 'effacer'
            , formatSubmit: 'yyyy/mm/dd'
        });
        $('.pickadate-format').pickadate({
            format: 'You selecte!d: dddd, dd mmm, yyyy'
            , formatSubmit: 'yyyy/mm/dd'
            , hiddenPrefix: 'prefix__'
            , hiddenSuffix: '__suffix'
        });
        $('.pickadate-editable').pickadate({
            editable: true
        });
        $('.pickadate-selectors').pickadate({
            selectYears: true
            , selectMonths: true
        });
        $('.pickadate-year').pickadate({
            selectYears: 4
        });
        $('.pickadate-weekday').pickadate({
            firstDay: 1
        });
        $('.pickadate-limits').pickadate({
            min: [2014, 3, 20]
            , max: [2014, 7, 14]
        });
        $('.pickadate-disable').pickadate({
            disable: [[2015, 8, 3], [2015, 8, 12], [2015, 8, 20]]
        });
        $('.pickadate-disable-range').pickadate({
            disable: [5, [2013, 10, 21, 'inverted'], {
                from: [2014, 3, 15]
                , to: [2014, 3, 25]
            }, [2014, 3, 20, 'inverted'], {
                from: [2014, 3, 17]
                , to: [2014, 3, 18]
                , inverted: true
            }]
        });
        $('.pickadate-events').pickadate({
            onStart: function () {
                console.log('Hello there :)')
            }
            , onRender: function () {
                console.log('Whoa.. rendered anew')
            }
            , onOpen: function () {
                console.log('Opened up')
            }
            , onClose: function () {
                console.log('Closed now')
            }
            , onStop: function () {
                console.log('See ya.')
            }
            , onSet: function (context) {
                console.log('Just set stuff:', context)
            }
        });
        $('.pickatime').pickatime();
        $('.pickatime-clear').pickatime({
            clear: ''
        });
        $('.pickatime-format').pickatime({
            format: 'T!ime selected: h:i a'
            , formatLabel: '<b>h</b>:i <!i>a</!i>'
            , formatSubmit: 'HH:i'
            , hiddenPrefix: 'prefix__'
            , hiddenSuffix: '__suffix'
        });
        $('.pickatime-hidden').pickatime({
            formatSubmit: 'HH:i'
            , hiddenName: true
        });
        $('.pickatime-editable').pickatime({
            editable: true
        });
        $('.pickatime-intervals').pickatime({
            interval: 150
        });
        $('.pickatime-limits').pickatime({
            min: [7, 30]
            , max: [14, 0]
        });


        $('.pickatime-disabled').pickatime({
            disable: [[0, 30], [2, 0], [8, 30], [9, 0]]
        });
        $('.pickatime-range').pickatime({
            disable: [1, [1, 30, 'inverted'], {
                from: [4, 30]
                , to: [10, 30]
            }, [6, 30, 'inverted'], {
                from: [8, 0]
                , to: [9, 0]
                , inverted: true
            }]
        });
        $('.pickatime-disableall').pickatime({
            disable: [true, 3, 5, 7, [0, 30], [2, 0], [8, 30], [9, 0]]
        });
        $('.pickatime-events').pickatime({
            onStart: function () {
                console.log('Hello there :)')
            }
            , onRender: function () {
                console.log('Whoa.. rendered anew')
            }
            , onOpen: function () {
                console.log('Opened up')
            }
            , onClose: function () {
                console.log('Closed now')
            }
            , onStop: function () {
                console.log('See ya.')
            }
            , onSet: function (context) {
                console.log('Just set stuff:', context)
            }
        });
    } catch (e) {

    }
    try {
        $("#anytime-date").AnyTime_picker({
            format: "%W, %M %D in the Year %z %E"
            , firstDOW: 1
        });
        $("#anytime-time").AnyTime_picker({
            format: "%H:%i"
        });
        $("#anytime-time-hours").AnyTime_picker({
            format: "%l %p"
        });
        $("#anytime-both").AnyTime_picker({
            format: "%M %D %H:%i"
        , });
        $("#anytime-weekday").AnyTime_picker({
            format: "%W, %D of %M, %Z"
        });
        $("#anytime-month-numeric").AnyTime_picker({
            format: "%d/%m/%Z"
        });
        $("#anytime-month-day").AnyTime_picker({
            format: "%D of %M"
        });
        $('#ButtonCreationDemoButton').click(function (e) {
            $('#ButtonCreationDemoInput').AnyTime_noPicker().AnyTime_picker().focus();
            e.preventDefault();
        });
        var oneDay = 24 * 60 * 60 * 1000;
        var rangeDemoFormat = "%e-%b-%Y";
        var rangeDemoConv = new AnyTime.Converter({
            format: rangeDemoFormat
        });
        $("#rangeDemoToday").click(function (e) {
            $("#rangeDemoStart").val(rangeDemoConv.format(new Date())).change();
        });
        $("#rangeDemoClear").click(function (e) {
            $("#rangeDemoStart").val("").change();
        });
        $("#rangeDemoStart").AnyTime_picker({
            format: rangeDemoFormat
        });
        $("#rangeDemoStart").change(function (e) {
            try {
                var fromDay = rangeDemoConv.parse($("#rangeDemoStart").val()).getTime();
                var dayLater = new Date(fromDay + oneDay);
                dayLater.setHours(0, 0, 0, 0);
                var ninetyDaysLater = new Date(fromDay + (90 * oneDay));
                ninetyDaysLater.setHours(23, 59, 59, 999);
                $("#rangeDemoFinish").AnyTime_noPicker().removeAttr("disabled").val(rangeDemoConv.format(dayLater)).AnyTime_picker({
                    earliest: dayLater
                    , format: rangeDemoFormat
                    , latest: ninetyDaysLater
                });
            } catch (e) {
                $("#rangeDemoFinish").val("").attr("disabled", "disabled");
            }
        });
    } catch (e) {

    }

});