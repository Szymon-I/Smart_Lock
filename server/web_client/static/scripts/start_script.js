$(document).ready(function () {


    let animate = Cookies.get('animation');
    // run animation on first startup
    const logo_text = "Smart Lock  ";
    let logo_count = 0;

    if (animate == "false") {
        $("#logo").html("Smart Lock  ");
        second_animation();
    } else {
        var typing = setInterval(function () {
            if (logo_count < logo_text.length) {
                $("#logo").html(logo_text.substring(0, logo_count + 1));
                logo_count++;
            } else {
                second_animation();
            }
        }, 250);
    }

    function second_animation() {
        if (typing) {
            clearInterval(typing);
        }
        $("#logo").append("<span class='oi oi-lock-locked custom_element animated slideInRight' id='lock_icon'></span>");
        $(".button_column").append("<a href='account/login'><button type='button' class='btn btn-outline-light animated fadeIn' style='margin-right:10px;'>Log In</button></a>");
        $(".button_column").append("<a href='account/signup'><button type='button' class='btn btn-outline-light animated fadeIn' style='margin-right:10px;'>Sign up</button></a>");
        $(".button_column").append("<button type='button' class='btn btn-outline-light animated fadeIn' id='learn_more'>?</button>");
        setTimeout(() => {
            $("#lock_icon").removeClass('slideInRight').addClass('swing');
        }, 2000);
        setInterval(() => {
            $("#lock_icon").removeClass('animated swing');
            setTimeout(() => {
                $("#lock_icon").addClass('animated swing');
            }, 200);
        }, 8000);
        Cookies.set('animation', 'false');
        $("#learn_more").click(function () {
            window.scrollTo({top: $('#learn_more_page').offset().top, behavior: 'smooth'});
            if ($("#learn_more_page").hasClass('invisible')) {
                $("#learn_more_page").removeClass('invisible');
                $(".container_info:eq(0)").removeClass('invisible');
                setTimeout(() => {
                    $(".container_info:eq(1)").removeClass('invisible');
                }, 1000);
                setTimeout(() => {
                    $(".container_info:eq(2)").removeClass('invisible');
                }, 2000);
                setTimeout(() => {
                    $(".container_info:eq(3)").removeClass('invisible');
                }, 3000);
                setTimeout(() => {
                    $(".page-footer").removeClass('invisible');
                }, 4000);
            }
            window.scrollTo({top: $('#learn_more_page').offset().top, behavior: 'smooth'});
        });
    }
});