# -*- mode: ruby -*-
# vi: set ft=ruby :

$ubuntuScript = <<-SCRIPT
  wget -q https://packages.microsoft.com/config/ubuntu/$(lsb_release -r -s)/packages-microsoft-prod.deb
  dpkg -i packages-microsoft-prod.deb
  sh -c 'echo "debug https://download.mono-project.com/repo/ubuntu stable-$(lsb_release -c -s) main" | > etc/apt/sources.list.d/mono.list'
  apt-key adv --keyserver apt-mo.trafficmanager.net --recv-keys 417A0893
  apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
  apt-get install apt-transport-https --no-install-recommends
  apt-get update
  apt-get install dotnet-sdk-2.2 mono-devel nuget npm ca-certificates-mono -y --no-install-recommends
  if [ -d "/home/vagrant" ]; then
    USER=vagrant
  elif [ -d "/home/ubuntu" ]; then
    USER=ubuntu
  fi
  source /home/${USER}/.profile
  [ -z "$DOTNET_CLI_TELEMETRY_OPTOUT" ] && echo "export DOTNET_CLI_TELEMETRY_OPTOUT=1" >> /home/${USER}/.profile
  [ -z "$DOTNET_SKIP_FIRST_TIME_EXPERIENCE" ] && echo "export DOTNET_SKIP_FIRST_TIME_EXPERIENCE" >> /home/${USER}/.profile
  exit 0
SCRIPT
Vagrant.configure("2") do |config|

  config.vm.define :trusty, autostart: false do |trusty|
    trusty.vm.box = "ubuntu/trusty64"
    trusty.vm.provision :shell, inline: $ubuntuScript
  end

  config.vm.define :xenial, autostart: false do |xenial|
    xenial.vm.box = "ubuntu/xenial64"
    xenial.vm.provision :shell, inline: 'echo "nameserver 8.8.8.8" > /etc/resolv.conf'
    xenial.vm.provision :shell, inline: $ubuntuScript
  end
end
