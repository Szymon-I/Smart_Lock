from django.contrib import admin
from .models import Lock, Log


class CustomLockAdmin(admin.ModelAdmin):
    model = Lock
    list_display = ['name', 'id', 'open_status']


class CustomLockLogAdmin(admin.ModelAdmin):
    model = Log
    list_display = ['get_name', 'id', 'action', 'date']

    def get_name(self, obj):
        return obj.lock.name


admin.site.register(Lock, CustomLockAdmin)
admin.site.register(Log, CustomLockLogAdmin)
