from django.db import models
from users.models import CustomUser
from django.utils.timezone import now


class Lock(models.Model):
    id = models.AutoField(primary_key=True)
    name = models.CharField(max_length=200)
    location = models.CharField(max_length=200)
    open_status = models.BooleanField(default=False)
    hardware = models.CharField(max_length=200)
    type = models.CharField(max_length=200)
    ip_address = models.GenericIPAddressField()
    user = models.ManyToManyField(CustomUser, blank=True)
    mac_adress = models.CharField(max_length=50)

    def __str__(self):
        return f'{self.id} -> {self.name}'


class Log(models.Model):
    lock = models.ForeignKey(Lock, on_delete=models.CASCADE)
    action = models.CharField(max_length=200)
    date = models.DateTimeField(default=now, blank=True)
    user = models.ForeignKey(CustomUser, on_delete=models.CASCADE)

    def __str__(self):
        return f'{self.id} -> {self.lock.name}'
