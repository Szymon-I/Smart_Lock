from django.urls import path, include
from .views import LoginView, LogoutView, GetLocks, LockAction
from rest_framework.authtoken import views

# this attaches signal for creating token
from .generate_token import *


urlpatterns = [

    path('login', LoginView.as_view(), name='login_view'),
    path('logout', LogoutView.as_view(), name='logout_view'),
    path('get_locks', GetLocks.as_view(), name='get_locks'),
    path('lock_action', LockAction.as_view(), name='lock_action'),

]
