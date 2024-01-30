$(document).ready(function () 
{
    // Enable Live Search.
    $('.selectPicker').attr('data-live-search', true);

    //// Enable multiple select.
    $('.selectPicker').attr('multiple', true);
    $('.selectPicker').attr('data-selected-text-format', 'count');

    $('.selectPicker').selectpicker(
    {
        width: '100%',
        title: '--All--',
        style: 'btn-warning',
        size: 6,
        iconBase: 'fa',
        tickIcon: 'fa-check'
    });
});  

