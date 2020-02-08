from rest_framework import serializers
from django.contrib.auth import authenticate
from rest_framework import exceptions
from users.models import CustomUser
from locks.models import Lock, Log


class LoginSerializer(serializers.Serializer):
    email = serializers.CharField()
    password = serializers.CharField()

    def validate(self, data):
        email = data.get("email", "")
        password = data.get("password", "")

        if email and password:
            user = authenticate(email=email, password=password)
            if user:
                if user.is_active:
                    data["user"] = user
                else:
                    msg = "User is deactivated."
                    raise exceptions.ValidationError(msg)
            else:
                msg = "Unable to login with given credentials."
                raise exceptions.ValidationError(msg)
        else:
            msg = "Must provide username and password both."
            raise exceptions.ValidationError(msg)
        return data


class LocksSerializer(serializers.ModelSerializer):
    class Meta:
        model = Lock
        fields = '__all__'


class LockActionSerializer(serializers.Serializer):
    id = serializers.IntegerField()
    action = serializers.CharField()

    def validate(self, data):
        choices = ['open', 'close', 'info']
        if data['action'] not in choices:
            raise exceptions.ValidationError("Wrong action")
        try:
            Lock.objects.get(id=data['id'])
        except Lock.DoesNotExist:
            raise exceptions.ValidationError("Wrong lock id")
        return data


class LockInfoSerializer(serializers.ModelSerializer):
    user = serializers.CharField(source='user.email', read_only=True)

    class Meta:
        model = Log
        fields = ['id', 'action', 'date', 'user']
