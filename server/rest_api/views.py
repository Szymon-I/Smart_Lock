from rest_framework.decorators import permission_classes
from rest_framework.views import APIView
from .serializers import LoginSerializer, LocksSerializer, LockActionSerializer
from rest_framework.authtoken.models import Token
from django.contrib.auth import login, logout
from rest_framework.response import Response
from rest_framework.authentication import TokenAuthentication
from rest_framework.permissions import AllowAny, IsAuthenticated
from locks.actions import *


@permission_classes((AllowAny,))
class LoginView(APIView):

    def post(self, request):
        serializer = LoginSerializer(data=request.data)
        serializer.is_valid(raise_exception=True)
        user = serializer.validated_data["user"]
        login(request, user)
        token, created = Token.objects.get_or_create(user=user)
        return Response({"token": token.key}, status=200)


class LogoutView(APIView):
    authentication_classes = (TokenAuthentication,)

    def post(self, request):
        logout(request)
        return Response(status=204)


class GetLocks(APIView):
    authentication_classes = [TokenAuthentication]
    permission_classes = [IsAuthenticated]

    def get(self, request, *args, **kwargs):
        content = LocksSerializer(Lock.objects.filter(user__id=request.user.id), many=True).data
        return Response(content)


class LockAction(APIView):
    authentication_classes = [TokenAuthentication]
    permission_classes = [IsAuthenticated]

    def post(self, request, *args, **kwargs):
        user = request.user
        serializer = LockActionSerializer(data=request.data)
        serializer.is_valid(raise_exception=True)
        print(serializer.data, serializer.data['id'])
        print(user.username)
        try:
            lock = Lock.objects.get(user__id=user.id, id=serializer.data['id'])
        except Lock.DoesNotExist:
            return Response({'content': 'Unauthorised action'}, status=401)
        if serializer.data['action'] in ('open', 'close'):
            status = lock_send(lock, user, serializer.data['action'])
            response = serializer.data
            if not status[0]:
                response['error'] = status[1]
                return Response(response, status=500)
            return Response(response, status=200)

        elif serializer.data['action'] == 'info':
            lock_logs = lock_info(lock)
            print(f"Lock (id={serializer.data['id']}) info")
            return Response(lock_logs, status=200)
