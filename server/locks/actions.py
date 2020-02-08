from locks.models import Lock, Log
import json
import requests
from rest_api.serializers import LockInfoSerializer
import hashlib
import os


def lock_send(lock, user, action):
    if not lock.open_status and action == 'open' or lock.open_status and action == 'close':
        try:
            action_pass = '1' if action == 'open' else '0'
            data = {
                'action': action_pass,
                'key': generate_key(lock.mac_adress)
            }
            print(data)
            r = send_node(data, lock)
            if "Error" in r.headers:
                return False, r.headers['Error']
            lock.open_status = True if action == 'open' else False
            lock.save()
            new_log = Log(lock=lock, action=action, user=user)
            new_log.save()
        except requests.exceptions.ConnectionError:
            print('error sending data do uC')
            return False, 'error sending data do uC'
    return True,


def lock_info(lock):
    logs = Log.objects.filter(lock=lock).order_by('-date')
    serializer = LockInfoSerializer(logs, many=True).data
    return serializer


def send_node(data, lock):
    url = f'http://{lock.ip_address}:8000/lock_action'
    payload = {
        'lock_action': data['action'],
        'key': data['key']
    }
    headers = {'content-type': 'application/json'}
    r = requests.post(url, data=json.dumps(payload), headers=headers, timeout=1)

    if "Error" in r.headers:
        print(f"Error: {r.headers['Error']}")
    print(r)
    return r


def generate_key(mac_adress):
    with open(os.path.join(os.getcwd(), 'locks', 'secret.key'), 'r') as f:
        secret = f.read()
    return hashlib.sha1(bytearray(secret + mac_adress, encoding='utf8')).hexdigest()
