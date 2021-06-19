import os
import time
import signal
import subprocess
import argparse

class ProcessConfig:
    def __init__(self, args: dict):
        self.base_dir = os.path.abspath(args["base_dir"])
        self.log_policy = args["log_policy"]
        self.profile = args["profile"]

class ProcessManager:
    def __init__(self, config: dict):
        self._terminate = False
        self._active_processes = {}
        self._status = {}
        self._open_files = []

        self._config = ProcessConfig(config)

    def _on_terminate(self, signum, frame):
        self._terminate = True
        print()
    
    def _print_status(self):
        term_cols, term_rows = os.get_terminal_size(0)
        print('\033[2J\033[H', end='')

        title = 'Application Orchestrator'
        print(f'\033[1m{title}\033[0m')
        print()

        for name, active in self._status.items():
            color = 32 if active else 31
            status = 'running' if active else 'stopped'
            colorize = lambda x: f'\033[{color}m{x}\033[0m'
            print(colorize(f'> {name}: {status}'), '\033[0K')
        print()

    def _open_log(self, name: str):
        path = f'{self._config.base_dir}/.logs/{name}.log'
        if self._config.log_policy == 'overwrite':
            logfile = open(path, 'w')
            self._open_files.append(logfile)
        elif self._config.log_policy == 'append':
            logfile = open(path, 'a')
            self._open_files.append(logfile)
        else:
            logfile = subprocess.DEVNULL
        return logfile

    def _close_all_files(self):
        for open_file in self._open_files:
            open_file.close()

    def _spawn_inner(self, name: str, *args):
        assert name not in self._status
        stdout = self._open_log(name)
        stderr = subprocess.STDOUT
        command = ' '.join(args)
        process = subprocess.Popen(command.split(), stdout=stdout, stderr=stderr)
        self._active_processes[name] = process
        self._status[name] = True
        return process

    def register_signals(self):
        signal.signal(signal.SIGINT, self._on_terminate)
        signal.signal(signal.SIGTERM, self._on_terminate)

    def spawn_process(self, name: str, command: str):
        process = self._spawn_inner(name, command)
        return process
    
    def spawn_dotnet(self, name: str, path: str):
        os.chdir(f'./{path}')
        profile_map = { 'dev': 'Development', 'test': 'Testing', 'prod': 'Production' }
        dotnet_profile = profile_map[self._config.profile]
        command = f'dotnet run --launch-profile={dotnet_profile}'
        process = self._spawn_inner(name, command)
        os.chdir('..')
        return process

    def _poll_unsafe(self):
        self._print_status()
        while self._active_processes:
            ended = []
            for name, proc in self._active_processes.items():
                if self._terminate:
                    proc.terminate()
                if proc.poll() is not None:
                    ended.append(name)
            for name in ended:
                self._status[name] = False
                del self._active_processes[name]
            if ended:
                self._print_status()
            time.sleep(0.2)
        self._close_all_files()

    def poll(self):
        try:
            self._poll_unsafe()
        except Exception as e:
            self._terminate = True
            self._poll_unsafe()
            raise

def parse_args():
    parser = argparse.ArgumentParser('central project runner')
    parser.add_argument('--profile', default='dev', choices=['dev', 'test', 'prod'])
    parser.add_argument('--log-policy', default='overwrite', choices=['none', 'overwrite', 'append']),
    parser.add_argument('--base-dir', default='.')
    return vars(parser.parse_args())

def main():
    proc_man = ProcessManager(parse_args())
    proc_man.register_signals()

    proc_man.spawn_dotnet('auth.api', 'StockMarket.Auth.Api')
    proc_man.spawn_dotnet('sector.api', 'StockMarket.Sector.Api')
    proc_man.spawn_dotnet('exchange.api', 'StockMarket.Exchange.Api')
    proc_man.spawn_dotnet('company.api', 'StockMarket.Company.Api')
    proc_man.spawn_dotnet('listing.api', 'StockMarket.Listing.Api')
    proc_man.spawn_dotnet('gateway.api', 'StockMarket.Gateway.Api')
    
    proc_man.poll()

main()