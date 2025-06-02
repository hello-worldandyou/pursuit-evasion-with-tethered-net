# 更新中...
# mlagent 官方说明文档
https://github.com/Unity-Technologies/ml-agents/tree/main/docs

# pursuit-evasion-with-tethered-net
Orbital TAD (1 attacker, 1 defender, 1 target)
links:

Orbital TAD (4 attacker, 1 defender, 1 target)
links:

Orbital TAD (4 attacker with a net, 1 defender, 1 target)
links: https://pan.baidu.com/s/14WDYcBPsIwX6jka9X3yGCQ 提取码: e4rt


# 运行说明
超参数yaml 文件参数说明链接：
https://zhuanlan.zhihu.com/p/393516577

生成Windows或ubuntu系统可执行文件，在mlagent Python环境下运行训练命令：
CUDA_VISIBLE_DEVICES=6 mlagents-learn SatellitePEOrig.yaml --run-id=beisu5fixedblue0522 --force --env SateP4exe/onlypursuer0522beisubluestatic.x86_64 --num-envs=10 --no-graphics --base-port=16322

CUDA_VISIBLE_DEVICES(选用哪个GPU)，SatellitePEOrig.yaml(yaml文件)， run-id(设置本次训练的ID)，  --force(重新训练，resume是继续训练)，SateP4exe/onlypursuer0522beisubluestatic.x86_64(训练文件的路径),
--num-envs(倍速训练，倍速效果不好)，--no-graphics(不要训练的动画界面)

# 代码说明

