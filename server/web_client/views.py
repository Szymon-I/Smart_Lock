from django.contrib.auth import authenticate, login
from django.shortcuts import render
from django.views.decorators.csrf import csrf_protect

from users.forms import CustomUserCreationForm, CustomUserLoginForm, RetrieveFrom
from django.http import HttpResponse, JsonResponse
from django.shortcuts import redirect
from django.contrib import messages
from allauth.account.views import PasswordResetDoneView
from locks.models import Lock
from locks.actions import *


# Create your views here.
def start_page(request):
    if request.user.is_authenticated:
        return dashboard(request)
    return render(request, 'index.html')


def login_page(request):
    if request.method == 'POST':
        form = CustomUserLoginForm(request, data=request.POST)
        if form.is_valid():
            username = form.cleaned_data.get('username')
            password = form.cleaned_data.get('password')
            user = authenticate(username=username, password=password)
            if user:
                login(request, user)
                messages.info(request, f'You are now logged in as: {username}')
                return redirect('start_page')
            else:
                messages.error(request, "Invalid username or password")

    form = CustomUserLoginForm
    return render(request, 'login.html', context={'form': form})


def register_page(request):
    if request.method == 'POST':
        form = CustomUserCreationForm(request, data=request.POST)
        if form.is_valid():
            email = form.cleaned_data.get('email')
            user = form.save()
            login(request, user)
            messages.info(request, f'You are now logged in as: {email}')
            return redirect('start_page')
        else:
            messages.error(request, "Invalid data to register")
    form = CustomUserCreationForm
    return render(request, 'register.html', context={'form': form})


def retrieve_password(request):
    if request.method == 'POST':
        messages.success(request, "We've send you an email to complete password reset")
        # tutaj wyslac mejla z resetem
        return redirect('start_page')

    form = RetrieveFrom
    return render(request, 'retrieve.html', context={'form': form})


def reset_done(PasswordResetDoneView):
    @property
    def success_url(self):
        return redirect('start_page')


def dashboard(request):
    locks = Lock.objects.filter(user=request.user).all()
    return render(request, 'dashboard.html', context={'locks': locks})


def lock_action(request):
    lock_id = request.POST.get('lock_id', None)
    action = request.POST.get('action', None)
    data = {'error': 'False'}

    try:
        lock = Lock.objects.get(user__id=request.user.id, id=lock_id)
        status = lock_send(lock, request.user, action)

    except Lock.DoesNotExist:
        data['error'] = 'Lock does not exist'
        return JsonResponse(data)
    if not status[0]:
        data['error'] = status[1]

    return JsonResponse(data)
