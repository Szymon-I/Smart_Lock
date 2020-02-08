from django.contrib import admin
from django.urls import path, include
from . import views

urlpatterns = [
    path('', views.start_page, name='start_page'),
    # path('login/', views.login_page, name='login_page'),
    # path('register/', views.register_page, name='register_page'),
    # path('retrieve/', views.retrieve_password, name='retrieve_password'),
    path('account/password/reset/done', views.reset_done, name='reset_done'),
    path('dashboard/', views.dashboard, name='dashboard'),
    path('lock_action/', views.lock_action, name='lock_action_web'),
]
