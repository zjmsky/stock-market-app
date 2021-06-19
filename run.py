import os
import time
import signal
import subprocess
import argparse

TERMINATE = False
PROCESSES = {}
STATUS = {}
OPEN_FILES = []

def terminate(signum, frame):
    global TERMINATE
    TERMINATE = True
    print()

def register_signals():
    signal.signal(signal.SIGINT, terminate)
    signal.signal(signal.SIGTERM, terminate)

def print_status():
    term_cols, term_rows = os.get_terminal_size(0)
    print("\033[2J\033[H", end="")

    title = "Application Orchestrator"
    print(f"\033[1m{title}\033[0m")
    print()

    for name, active in STATUS.items():
        color = 32 if active else 31
        status = "running" if active else "stopped"
        colorize = lambda x: f"\033[{color}m{x}\033[0m"
        print(colorize(f"> {name}: {status}"), '\033[0K')
    print()

def poll_processes():
    print_status()
    while PROCESSES:
        ended = []
        for name, proc in PROCESSES.items():
            if TERMINATE:
                proc.terminate()
            if proc.poll() is not None:
                ended.append(name)
        for name in ended:
            STATUS[name] = False
            del PROCESSES[name]
        if ended:
            print_status()
        time.sleep(0.2)

def close_all_files():
    for open_file in OPEN_FILES:
        open_file.close()

def register_process(function):
    def inner(name, *args, **kwargs):
        if name in STATUS:
            print(f"ERROR: duplicate {name} invocation")
            return
        process = function(name, *args, **kwargs)
        PROCESSES[name] = process
        STATUS[name] = True
    return inner

def restore_dir(function):
    def inner(*args, **kwargs):
        cwd = os.getcwd()
        result = function(*args, **kwargs)
        os.chdir(cwd)
        return result
    return inner

@register_process
def start_process(name: str, *args):
    logfile = open(f'./.logs/{name}.log', 'a')
    OPEN_FILES.append(logfile)
    args = ' '.join(args).split()
    return subprocess.Popen(args, stdout=logfile, stderr=subprocess.STDOUT)

@restore_dir
@register_process
def start_dotnet_project(name: str, path: str, profile: str):
    logfile = open(f'./.logs/{name}.log', 'a')
    OPEN_FILES.append(logfile)
    os.chdir(f'./{path}')
    command = f'dotnet run --launch-profile={profile}'
    return subprocess.Popen(command.split(), stdout=logfile, stderr=subprocess.STDOUT)

def parse_args():
    parser = argparse.ArgumentParser('central project runner')
    parser.add_argument('-p', '--profile', default="Development")
    return parser.parse_args()

def main():
    args = parse_args()

    register_signals()

    # api microservices
    start_dotnet_project("auth.api", 'StockMarket.Auth.Api', args.profile)
    start_dotnet_project("sector.api", 'StockMarket.Sector.Api', args.profile)
    start_dotnet_project("exchange.api", 'StockMarket.Exchange.Api', args.profile)
    start_dotnet_project("company.api", 'StockMarket.Company.Api', args.profile)
    start_dotnet_project("listing.api", 'StockMarket.Listing.Api', args.profile)
    start_dotnet_project("gateway.api", 'StockMarket.Gateway.Api', args.profile)
    
    poll_processes()

def safe_main():
    global TERMINATE
    try:
        main()
    except Exception as e:
        TERMINATE = True
        poll_processes()
        raise

safe_main()