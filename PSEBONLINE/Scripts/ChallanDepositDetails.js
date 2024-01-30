    $(document).ready(function () {
        $("#divP").hide();
        $("#p1").hide();
        $('#ddlChallanId').change(function () {
           //  alert('1');
             var selectedChallanId = $("#ddlChallanId").val();
           //  alert(selectedChallanId);
            if (selectedChallanId != "") {
                $('#loading1').html("<p><img src='https://registration2022.pseb.ac.in/images/loadingk.gif'></p>");
                $.post("/Home/GetChallanDetails", { "challanid": selectedChallanId },
                                          function (results) {
                                              //alert('end');
                                             // alert('data.status  = ' + results.status);
                                              if (results.status == 1) {                                                 
                                                 // alert('bind start');
                                                  $("#divP").show();
                                                  $('#loading1').empty();
                                                //  alert(results.chid);
                                                  $("#CHALLANID").val(results.chid)
                                                  $('#SCHLREGID').val(results.SCHLREGID);
                                                  $('#LOT').val(results.LOT);
                                                  $('#FEE').val(results.FEE);
                                                  $('#BANK').val(results.BANK);
                                                  $("#APPNO").val(results.APPNO)
                                                  $('#SCHLCANDNM').val(results.SCHLCANDNM);
                                                  $('#CHLNDATE').val(results.CHLNDATE);
                                                  $('#CHLVDATE').val(results.CHLVDATE);
                                                  $('#DOWNLDDATE').val(results.DOWNLDFSTATUS);
                                                  $('#BRANCHCAND').val(results.BRANCHCAND);
                                                  $('#BRCODECAND').val(results.BRCODECAND);
                                                  $('#J_REF_NOCAND').val(results.J_REF_NOCAND);
                                                  $('#DEPOSITDTCAND').val(results.DEPOSITDTCAND);
                                                  $('#DEPOSITDTCANDR').val(results.DEPOSITDTCAND);

                                                  $("#CHALLANID").prop("readonly", true);
                                                  $("#SCHLREGID").prop("readonly", true);
                                                  $("#FEE").prop("readonly", true);
                                                  $("#BANK").prop("readonly", true);
                                                  $("#LOT").prop("readonly", true);

                                                  $("#APPNO").prop("readonly", true);
                                                  $("#SCHLCANDNM").prop("readonly", true);
                                                  $("#CHLNDATE").prop("readonly", true);
                                                  $("#CHLVDATE").prop("readonly", true);
                                                  $("#DOWNLDDATE").prop("readonly", true);

                                                  if (Session["AdminId"] != null) {
                                                      $("#BRANCHCAND").prop("readonly", false);
                                                      $("#BRCODECAND").prop("readonly", false);
                                                      $("#J_REF_NOCAND").prop("readonly", false);
                                                      $("#DEPOSITDTCAND").prop("readonly", false);
                                                      $("#DEPOSITDTCANDR").prop("readonly", false);
                                                      $("#Submit").show();
                                                      $("#p1").hide();
                                                      $("#DEPOSITDTCAND").addClass('date');
                                                      $("#DEPOSITDTCAND").show();
                                                      $("#DEPOSITDTCANDR").hide();

                                                  }
                                                  else {
                                                      if (results.J_REF_NOCAND != "") {
                                                          $("#BRANCHCAND").prop("readonly", true);
                                                          $("#BRCODECAND").prop("readonly", true);
                                                          $("#J_REF_NOCAND").prop("readonly", true);
                                                          $("#DEPOSITDTCAND").prop("readonly", true);
                                                          $("#DEPOSITDTCANDR").prop("readonly", true);
                                                          $("#Submit").hide();
                                                          $("#p1").show();
                                                          $("#DEPOSITDTCAND").hide();
                                                          $("#DEPOSITDTCANDR").show();

                                                      }
                                                      else {
                                                          $("#BRANCHCAND").prop("readonly", false);
                                                          $("#BRCODECAND").prop("readonly", false);
                                                          $("#J_REF_NOCAND").prop("readonly", false);
                                                          $("#DEPOSITDTCAND").prop("readonly", false);
                                                          $("#DEPOSITDTCANDR").prop("readonly", false);
                                                          $("#Submit").show();
                                                          $("#p1").hide();
                                                          $("#DEPOSITDTCAND").addClass('date');
                                                          $("#DEPOSITDTCAND").show();
                                                          $("#DEPOSITDTCANDR").hide();
                                                      }
                                                  }

                                              }
                                              else {
                                                  $("#p1").hide();
                                                  $("#divP").hide();
                                                  $('#loading1').hide();
                                                  alert("Please Try Again");
                                              }
                                          });
            }
            else {
                alert('Please Selected ChallanId');
                $("#divP").hide();
                $("#p1").hide();
            }
        });
    });


    function formVal() {
       //   alert('check formVal');
        var BRCODECAND = document.getElementById('BRCODECAND').value;
       // alert(BRCODECAND);

        var BRANCHCAND = document.getElementById('BRANCHCAND').value;
     //   alert(BRANCHCAND);
        var J_REF_NOCAND = document.getElementById('J_REF_NOCAND').value;
      //   alert(J_REF_NOCAND);
        var DEPOSITDTCAND = document.getElementById('DEPOSITDTCAND').value;
      //  alert("DEPOSITDTCAND : " + DEPOSITDTCAND);
        var CHALLANID = document.getElementById('CHALLANID').value;
      //   alert(CHALLANID);  

        var CHLNDATE = document.getElementById('CHLNDATE').value;
      //  alert("CHLNDATE : " + CHLNDATE);
        var CHLVDATE = document.getElementById('CHLVDATE').value;
       // alert("CHLVDATE : " + CHLVDATE);
        var SCHLCANDNM = document.getElementById('SCHLCANDNM').value;

        var dateFirst = new Date();
        var dateSecond = new Date();
        var dateThird = new Date();
        dateFirst = CHLNDATE.split('/');
        dateSecond = CHLVDATE.split('/');
        dateThird = DEPOSITDTCAND.split('/');
        var dGen = new Date(dateFirst[2], dateFirst[1], dateFirst[0]); //Year, Month, Date
        var dVAL = new Date(dateSecond[2], dateSecond[1], dateSecond[0]);
        var dDep = new Date(dateThird[2], dateThird[1], dateThird[0]);

       // alert(dGen);
      //  alert(dVAL);
      //  alert(dDep);
        if (dDep < dGen) {
            alert("Enter Valid Deposit Date...Deposit Date cannot be less than  Challan Generated Date");
            document.getElementById('DEPOSITDTCAND').focus();
            return false;
        }
        else if (dDep > dVAL) {
            alert("Enter Valid Deposit Date...Deposit Date cannot be greater than to Challan Valid Date");
            document.getElementById('DEPOSITDTCAND').focus();
            return false;
        }

        else if (CHALLANID == '') {
            alert("Enter CHALLANID.");
            document.getElementById('CHALLANID').focus();
            return false;
        }       
        else if (BRANCHCAND == '') {
            alert("Enter Bank Branch Name .");
            document.getElementById('BRANCHCAND').focus();
            return false;
        }
        else if (J_REF_NOCAND == '') {
            alert("Enter Bank Reference No");
            document.getElementById('J_REF_NOCAND').focus();
            return false;
        }
       else  if (DEPOSITDTCAND == '') {
           alert("Enter Fee Deposit Date ");
            document.getElementById('DEPOSITDTCAND').focus();
            return false;
        }
       else {          
          alert("Check Following Details, If correct then click Ok button otherwise click Cancel button to correct details.?");           
           $('#dCHALLANID').val(CHALLANID);
           $('#dSCHLCANDNM').val(SCHLCANDNM);
           $('#dBRCODECAND').val(BRCODECAND);
           $('#dBRANCHCAND').val(BRANCHCAND);
           $('#dJ_REF_NOCAND').val(J_REF_NOCAND);
           $('#dDEPOSITDTCAND').val(DEPOSITDTCAND);
           $("#dCHALLANID").prop("readonly", true);
           $("#dSCHLCANDNM").prop("readonly", true);
           $("#dBRCODECAND").prop("readonly", true);
           $("#dBRANCHCAND").prop("readonly", true);
           $("#dJ_REF_NOCAND").prop("readonly", true);
           $("#dDEPOSITDTCAND").prop("readonly", true);
         //  alert('start open');
           $('#dialog').dialog('open');
         //  alert('end open');
           return true;
       }
    }




    function formValAdmin() {
        //   alert('check formVal');
        var BRCODECAND = document.getElementById('BRCODECAND').value;
        // alert(BRCODECAND);

        var BRANCHCAND = document.getElementById('BRANCHCAND').value;
        //   alert(BRANCHCAND);
        var J_REF_NOCAND = document.getElementById('J_REF_NOCAND').value;
        //   alert(J_REF_NOCAND);
        var DEPOSITDTCAND = document.getElementById('DEPOSITDTCAND').value;
        //  alert("DEPOSITDTCAND : " + DEPOSITDTCAND);
        var CHALLANID = document.getElementById('CHALLANID').value;
        //   alert(CHALLANID);  

        var CHLNDATE = document.getElementById('CHLNDATE').value;
        //  alert("CHLNDATE : " + CHLNDATE);
        var CHLVDATE = document.getElementById('CHLVDATE').value;
        // alert("CHLVDATE : " + CHLVDATE);
        var SCHLCANDNM = document.getElementById('SCHLCANDNM').value;

        var dateFirst = new Date();
        var dateSecond = new Date();
        var dateThird = new Date();
        dateFirst = CHLNDATE.split('/');
        dateSecond = CHLVDATE.split('/');
        dateThird = DEPOSITDTCAND.split('/');
        var dGen = new Date(dateFirst[2], dateFirst[1], dateFirst[0]); //Year, Month, Date
        var dVAL = new Date(dateSecond[2], dateSecond[1], dateSecond[0]);
        var dDep = new Date(dateThird[2], dateThird[1], dateThird[0]);

        var challanremarks = document.getElementById('challanremarks').value;
      
        if (dDep < dGen) {
            alert("Enter Valid Deposit Date...Deposit Date cannot be less than  Challan Generated Date");
            document.getElementById('DEPOSITDTCAND').focus();
            return false;
        }
        else if (dDep > dVAL) {
            alert("Enter Valid Deposit Date...Deposit Date cannot be greater than to Challan Valid Date");
            document.getElementById('DEPOSITDTCAND').focus();
            return false;
        }

        else if (CHALLANID == '') {
            alert("Enter CHALLANID.");
            document.getElementById('CHALLANID').focus();
            return false;
        }   
        else if (challanremarks == '') {
            alert("Enter Remarks.");
            document.getElementById('challanremarks').focus();
            return false;
        }   
        
        else {
            alert("Check Following Details, If correct then click Ok button otherwise click Cancel button to correct details.?");
            $('#dCHALLANID').val(CHALLANID);
            $('#dSCHLCANDNM').val(SCHLCANDNM);
            $('#dBRCODECAND').val(BRCODECAND);
            $('#dBRANCHCAND').val(BRANCHCAND);
            $('#dJ_REF_NOCAND').val(J_REF_NOCAND);
            $('#dDEPOSITDTCAND').val(DEPOSITDTCAND);
            $('#dChallanremarks').val(challanremarks);
            $("#dCHALLANID").prop("readonly", true);
            $("#dSCHLCANDNM").prop("readonly", true);
            $("#dBRCODECAND").prop("readonly", true);
            $("#dBRANCHCAND").prop("readonly", true);
            $("#dJ_REF_NOCAND").prop("readonly", true);
            $("#dDEPOSITDTCAND").prop("readonly", true);
            $("#dChallanremarks").prop("readonly", true);
            //  alert('start open');
            $('#dialog').dialog('open');
            //  alert('end open');
            return true;
        }
    }


    $(function () {
        $("#dialog").dialog({
            modal: true,
            autoOpen: false,
            title: "Confirmation Details ?",
            width: 800,
            height: 420,
            buttons: {
                "Confirm": function () {
                    $('#myForm').submit();                    
                   // window.location.href = '@Url.Action("ChallanDepositDetails", "Home", new {id = @ViewBag.cid })';
                },
                Cancel: function () {
                    $(this).dialog("close");
                }
            }
        });
        $("#btnShow").click(function () {
            $('#dialog').dialog('open');
        });
    });